angular.module("umbraco").controller("splatDev.SiteBackup.Controller", function ($scope, dialogService) {
        'use strict';
    const vm = this;
       
        function init() {
            if ($scope.model.value !== undefined && $scope.model.value !== '') vm.model = $scope.model.value;
        }
        init();
    });