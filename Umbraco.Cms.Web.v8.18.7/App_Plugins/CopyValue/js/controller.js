angular.module("umbraco").controller("splatDev.CopyValue.Controller", function ($scope, $routeParams, contentEditingHelper, editorState) {
    'use strict';
    const vm = this;
    vm.from = $scope.model.config.from.split(",");
    vm.to = $scope.model.config.to.split(",")

    var languages = $scope.model.languages;
   
    if (languages == null) {
        let content = editorState.current.variants.find(x => x.language === $routeParams.mculture);
        let properties = contentEditingHelper.getAllProps(content);
        vm.done = false;

        vm.copy = function () {
            for (var i = 0; i < vm.from.length; i++) {
                _.findWhere(properties, { alias: vm.to[i] }).value = _.findWhere(properties, { alias: vm.from[i] }).value;
            }

            vm.done = true;
            setTimeout(function () {
                vm.done = false;
            }, 3 * 1000);
    }

    /*if ($scope.model.language != null) {
        let content = editorState.current.variants.find(x => x.language.culture === $routeParams.mculture);
        let properties = contentEditingHelper.getAllProps(content);
        vm.done = false;

        vm.copy = function () {
            for (var i = 0; i < vm.from.length; i++) {
                _.findWhere(properties, { alias: vm.to[i] }).value = _.findWhere(properties, { alias: vm.from[i] }).value;
            }

            vm.done = true;
            setTimeout(function () {
                vm.done = false;
            }, 3 * 1000);
        };*/
    } else {
        let content = editorState.current;
        let properties = contentEditingHelper.getAllProps(content);
        vm.done = false;

        vm.copy = function () {
            for (var i = 0; i < vm.from.length; i++) {
                _.findWhere(properties, { alias: vm.to[i] }).value = _.findWhere(properties, { alias: vm.from[i] }).value;
            }

            vm.done = true;
            setTimeout(function () {
                vm.done = false;
            }, 3 * 1000);
        };
	}
        
   
});