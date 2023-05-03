angular.module("umbraco").config(function ($sceDelegateProvider) {
	$sceDelegateProvider.trustedResourceUrlList([
		'self',
		'https://*.youtube.com/**'
	]);
})
angular.module("umbraco").controller("splatDev.YouTubePreview.Controller", function ($scope) {
	'use strict';
	const vm = this;
	vm.videoId = null;
	$scope.getIframeSrc = function (videoId) {
		return 'https://www.youtube.com/embed/' + videoId;
	};

	/*vm.update = function () {
	    
		$scope.model.value = vm.videoId
	};*/

	function init() {
		if ($scope.model.value !== "") {

			vm.videoId = $scope.model.value;
		}
	}

	init()
});