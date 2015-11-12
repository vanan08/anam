//'use strict';

app.controller('ProductController', function ($scope, $http, $modal, toaster) {
    $scope.pagingInfo = {
        page: 1,
        pageSize: 10,
        sortBy: 'productid',
        isAsc: true,
        totalItems: 0
    };

    $scope.products = null;

    $scope.gridIndex = function (index) {
        return (index + 1) + (($scope.pagingInfo.page - 1) * $scope.pagingInfo.pageSize);
    };

    $scope.setPage = function (pageNo) {
        $scope.pagingInfo.page = pageNo;
    };

    $scope.pageChanged = function () {
        pageInit();
    };

    $scope.sort = function (sortBy) {
        if (sortBy === $scope.pagingInfo.sortBy) {
            $scope.pagingInfo.isAsc = !$scope.pagingInfo.isAsc;
        } else {
            $scope.pagingInfo.sortBy = sortBy;
            $scope.pagingInfo.isAsc = false;
        }
        pageInit();
    };

    $scope.openModal = function (id, html) {
        var modalInstance = $modal.open({
            backdrop: 'static',
            templateUrl: html,
            controller: 'ModalController',
            size: '',
            resolve: {
                selectedID: function () {
                    return id;
                },
                selectedPagingInfo: function () {
                    return $scope.pagingInfo;
                }
            }
        });

        modalInstance.result.then(function (refreshProducts) {
            $scope.products = refreshProducts;
        }, function () {
            //$log.info('Modal dismissed at: ' + new Date());
        });
    };

    function loadTotalItems() {
        $http.get('/api/product/getsize')
        .success(function (data, status, headers, config) {
            $scope.pagingInfo.totalItems = data;
        })
        .error(function (data, status, headers, config) {
            $scope.error = "Error has occured!";
            toaster.pop('error', $scope.error);
        });
    }

    function loadProducts() {
        $http.get('/api/product/getallby', { params: $scope.pagingInfo })
        .success(function (data, status, headers, config) {
            $scope.products = data;
        })
        .error(function (data, status, headers, config) {
            $scope.error = "Error has occured!";
            toaster.pop('error', $scope.error);
        });
    }

    function pageInit() {
        loadTotalItems();
        loadProducts();
        //toaster.pop('info', 'Load page ' + $scope.pagingInfo.page + ' and sort by ' + $scope.pagingInfo.sortBy);
    }

    pageInit();
});

app.controller('ModalController', function ($scope, $modalInstance, $http, selectedID, selectedPagingInfo, toaster) {
    $scope.selectedID = selectedID;
    $scope.selectedPagingInfo = selectedPagingInfo;

    $scope.ok = function () {
        closeAndRefreshRepeater();
        //$modalInstance.close();
    };

    $scope.cancel = function () {
        closeAndRefreshRepeater();
        //$modalInstance.dismiss('cancel');
    };

    $scope.create = function (product) {
        if ($scope.createForm.$valid) {
            $scope.isProcessing = true;
            $http({
                method: 'POST',
                url: '/api/product/create',
                data: {
                    'ProductName': product.ProductName,
                    'SupplierID': product.SupplierID,
                    'CategoryID': product.CategoryID,
                    'QuantityPerUnit': product.QuantityPerUnit,
                    'UnitPrice': product.UnitPrice,
                    'UnitsInStock': product.UnitsInStock,
                    'UnitsOnOrder': product.UnitsOnOrder,
                    'ReorderLevel': product.ReorderLevel,
                    'Discontinued': product.Discontinued
                },
                headers: { 'Content-Type': 'application/json' }
            })
            .success(function (data, status, headers, config) {
                closeAndRefreshRepeater();
                $scope.isProcessing = false;
                toaster.pop('success', 'Create successful...');
            })
            .error(function (data, status, headers, config) {
                $scope.error = "Error has occured!";
                toaster.pop('error', $scope.error);
            });
        }
        else {
        }
    };

    $scope.update = function (product) {
        if ($scope.editForm.$valid) {
            $scope.isProcessing = true;
            $http({
                method: 'PUT',
                url: '/api/product/edit/' + product.ProductID,
                data: {
                    'ProductID': product.ProductID,
                    'ProductName': product.ProductName,
                    'SupplierID': product.SupplierID,
                    'CategoryID': product.CategoryID,
                    'QuantityPerUnit': product.QuantityPerUnit,
                    'UnitPrice': product.UnitPrice,
                    'UnitsInStock': product.UnitsInStock,
                    'UnitsOnOrder': product.UnitsOnOrder,
                    'ReorderLevel': product.ReorderLevel,
                    'Discontinued': product.Discontinued
                },
                headers: { 'Content-Type': 'application/json' }
            })
            .success(function (data, status, headers, config) {
                closeAndRefreshRepeater();
                $scope.isProcessing = false;
                toaster.pop('success', 'Update successful...');
            })
            .error(function (data, status, headers, config) {
                $scope.error = "Error has occured!";
                toaster.pop('error', $scope.error);
            });
        }
        else {
        }
    };

    $scope.delete = function (id) {
        $scope.isProcessing = true;
        $http.delete('/api/product/delete', {
            params: { 'id': id }
        })
        .success(function (data, status, headers, config) {
            closeAndRefreshRepeater();
            $scope.isProcessing = false;
            toaster.pop('success', 'Delete successful...');
        })
        .error(function (data, status, headers, config) {
            $scope.error = "Error has occured!";
            toaster.pop('error', $scope.error);
        });
    };

    function getAvailableSuppliers() {
        $scope.availableSuppliers = [];
        $http.get('/api/supplier/getall')
        .success(function (data, status, headers, config) {
            $.each(data, function (index, item) {
                $scope.availableSuppliers.push({ ID: item.SupplierID, Name: item.ContactName + ' (' + item.CompanyName + ')' });
            });
        })
        .error(function () {
            $scope.error = "Error has occured!";
            toaster.pop('error', $scope.error);
        });
    }

    function getAvailableCategories() {
        $scope.availableCategories = [];
        $http.get('/api/category/getall')
        .success(function (data, status, headers, config) {
            $.each(data, function (index, item) {
                $scope.availableCategories.push({ ID: item.CategoryID, Name: item.CategoryName });
            });
        })
        .error(function (data, status, headers, config) {
            $scope.error = "Error has occured!";
            toaster.pop('error', $scope.error);
        });
    }

    function loadProduct() {
        $scope.product = null;
        $http.get('/api/product/get', { params: { id: $scope.selectedID } })
        .success(function (data, status, headers, config) {
            $scope.product = data;
        })
        .error(function (data, status, headers, config) {
            $scope.error = "Error has occured!";
            toaster.pop('error', $scope.error);
        });
    }

    function loadTotalItems() {
        $http.get('/api/product/getsize')
        .success(function (data, status, headers, config) {
            $scope.selectedPagingInfo.totalItems = data;
        })
        .error(function (data, status, headers, config) {
            $scope.error = "Error has occured!";
            toaster.pop('error', $scope.error);
        });
    }

    function loadProducts() {
        $http.get('/api/product/getallby', { params: $scope.selectedPagingInfo })
        .success(function (data, status, headers, config) {
            $scope.refreshProducts = data;
            $modalInstance.close(data);
        })
        .error(function (data, status, headers, config) {
            $scope.error = "Error has occured!";
            toaster.pop('error', $scope.error);
        });
    }

    function closeAndRefreshRepeater() {
        loadTotalItems();
        loadProducts();
    }

    function pageInit() {
        getAvailableSuppliers();
        getAvailableCategories();

        if ($scope.selectedID != '')
            loadProduct();

    }

    pageInit();
});