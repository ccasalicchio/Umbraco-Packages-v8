angular.module("umbraco").controller("splatDev.CopyValue.Controller", function ($scope, contentEditingHelper, editorState) {
    'use strict';
    const vm = this;
    vm.from = $scope.model.config.from.split(",");
    vm.to = $scope.model.config.to.split(",")

    vm.display = {
        from: [],
        to: []
    }

    let content = editorState.current.variants.length === 1 ? editorState.current.variants[0] : editorState.current.variants(x => x.language != null);
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

    vm.$onInit = function () {
        for (var i = 0; i < vm.from.length; i++) {
            vm.display.from.push(_.findWhere(properties, { alias: vm.from[i] }).label);
            vm.display.to.push(_.findWhere(properties, { alias: vm.to[i] }).label);
        }
    }
});