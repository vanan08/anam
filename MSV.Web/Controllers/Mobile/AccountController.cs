using Accuracy.Core.Repositories;
using Accuracy.Web.Common;
using Accuracy.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Accuracy.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Accuracy.Web.AutRepository;

namespace Accuracy.Web.Controllers.API
{
    /// <summary>
    /// Bao gồm các APIs: Đăng ký bằng email, facebook và làm mới token
    /// </summary>
    [RoutePrefix("api/Account")]
    public class AccountController : ApiController
    {
        private AuthRepository _repo = null;
        private IRepository<Image> ImageRepository = new Repository<Image>();
        private IRepository<UserImage> UserImageRepository = new Repository<UserImage>();
        /// <summary>
        /// 
        /// </summary>
        public AccountController()
            : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext())))
        {
            _repo = new AuthRepository();
        }

        /// <summary>
        /// Hàm khởi tạo của Account controller
        /// </summary>
        /// <param name="userManager"></param>
        public AccountController(UserManager<ApplicationUser> userManager)
        {
            UserManager = userManager;
        }


        /// <summary>
        /// Thuộc tính UserManager để tương tác với OWin User Repository
        /// </summary>
        public UserManager<ApplicationUser> UserManager { get; private set; }

        //  POST api/Account/Login
        /// <summary>
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("Login")]
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return null;
            }

            var user = await UserManager.FindByNameAsync(model.Email);
            int[] myInts = model.ImageIndexs.Split(';').Select(int.Parse).ToArray();

            if (user != null)
            {
                var userImages = await UserImageRepository.FindAllByAsync(u => u.Username == model.Email);
                int[] ImageIds = new int[userImages.ToArray().Count()];
                for (int i = 0; i < userImages.ToArray().Count(); i++)
                {
                    ImageIds[i] = userImages.ToArray()[i].ImageId;
                }

                var images = await ImageRepository.FindAllAsync();
                var imageModels = images.Select(x => new ImageModel
                {
                    Id = x.Id,
                    ImageUrl = x.ImageUrl,
                    Index = x.Index
                }).Where(z => ImageIds.Contains(z.Id)).Where(z => myInts.Contains(z.Index)).OrderBy(x => x.Index);
                if (imageModels.Count() == 3)
                    return Ok(imageModels);
            }

            return Ok("Fail");
        }

        // POST api/Account/Register
        /// <summary>
        /// Đăng ký bằng email
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("Register")]
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> Register(RegisterViewModel model)
        {
            string clientId = string.Empty;
            string clientSecret = string.Empty;
            if (!ModelState.IsValid)
            {
                return Ok(ModelState);
            }

            var user = await UserManager.FindByNameAsync(model.Email);
            if (user != null)
            {
                return Ok("User is exists.");
            }

            user = new ApplicationUser() { 
                UserName = model.Email, 
                Email = model.Email
            };
            var result = await UserManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var images = await ImageRepository.FindAllAsync();

                int[] myInts = model.ImageIndexs.Split(';').Select(int.Parse).ToArray();

                var imageModels = images.Select(x => new ImageModel
                {
                    Id = x.Id,
                    ImageUrl = x.ImageUrl,
                    Index = x.Index
                }).Where(z => myInts.Contains(z.Index)).OrderBy(x => x.Index).ToArray();

                for(int i=0; i<imageModels.Count(); i++){
                    int imageId = imageModels.ToArray()[i].Id;
                    await UserImageRepository.SaveAsync(new UserImage() { ImageId = imageId, Username = model.Email });
                }
                return Ok("Registered");
            }

            return Ok("Register fail");
        }


        [AllowAnonymous]
        [Route("GetRegisterImages")]
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> GetRegisterImages(LoginViewModel model)
        {
            if (model.Times > 3)
                return Ok("Ok");
            try
            {
                var images = await ImageRepository.FindAllAsync();
                int count = images.Select(x => new ImageModel
                {
                    Id = x.Id,
                    ImageUrl = x.ImageUrl,
                    Index = x.Index
                }).Count();

                int[] indexArray = new int[9];
                Random getrandom = new Random();
                var values = Enumerable.Range(1, count).OrderBy(x => getrandom.Next()).ToArray();
                for (int i = 0; i < indexArray.Length; i++)
                {
                    indexArray[i] = values[i];
                }


                return Ok(images.Select(x => new ImageModel
                {
                    Id = x.Id,
                    ImageUrl = x.ImageUrl,
                    Index = x.Index
                }).Where(z => indexArray.Contains(z.Index)).OrderBy(x => x.Index).ToArray());
            }
            catch (Exception ex)
            {

            }
            return Ok("Fail");
            
        }


        [AllowAnonymous]
        [Route("GetLoginImages")]
        [System.Web.Http.HttpPost]
        public async Task<IHttpActionResult> GetLoginImages(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return null;
            }

            var user = await UserManager.FindByNameAsync(model.Email);

            if (user != null)
            {
                if (model.Times > 3)
                    return Ok("Ok");

                var images = await ImageRepository.FindAllAsync();
                int count = images.Select(x => new ImageModel
                {
                    Id = x.Id,
                    ImageUrl = x.ImageUrl,
                    Index = x.Index
                }).Count();

                

                //Get all images user registed
                var userImages = await UserImageRepository.FindAllByAsync(u => u.Username == model.Email);
                var arrUserImages = userImages.ToArray();

                List<int> lsImageIds = new List<int>();
                for (int i = 0; i < arrUserImages.Length; i++)
                {
                    lsImageIds.Add(arrUserImages[i].ImageId);
                }

                int[] indexArray = new int[9];
                Random getrandom = new Random();
                var values = Enumerable.Range(1, count).OrderBy(x => getrandom.Next()).ToArray();
                List<int> iValues = new List<int>();
                for (int i = 0; i < values.Length; i++)
                {
                    if (!lsImageIds.Contains(values[i]))
                    {
                        iValues.Add(values[i]);
                    }
                }

                for (int i = 0; i < indexArray.Length; i++)
                {
                    indexArray[i] = iValues[i];
                }

                //Get a image (3times)
                UserImage userImage = arrUserImages[model.Times - 1];
                ImageModel imageModel = images.Select(x => new ImageModel
                {
                    Id = x.Id,
                    ImageUrl = x.ImageUrl,
                    Index = x.Index
                }).Where(z => z.Id == userImage.ImageId).FirstOrDefault();

                //Get random index for set user image registed to array
                int userSelectedIndex = getrandom.Next(1, 9);
                

                var imageArray = images.Select(x => new ImageModel
                {
                    Id = x.Id,
                    ImageUrl = x.ImageUrl,
                    Index = x.Index
                }).Where(z => indexArray.Contains(z.Index)).Where(z => !lsImageIds.Contains(z.Id)).OrderBy(x => x.Index);


                ImageModel[] lsImageModel = imageArray.ToArray();
                lsImageModel[userSelectedIndex] = imageModel;
                return Ok(lsImageModel);
            }

            return Ok("Fail");
        }

      

       /// <summary>
       /// 
       /// </summary>
       /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _repo.Dispose();
            }

            base.Dispose(disposing);
        }        

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        
    }
}