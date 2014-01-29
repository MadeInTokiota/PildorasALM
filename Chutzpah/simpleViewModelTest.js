/// <reference path="simpleViewModel.js" />
test("Sumar dos números", function () {
    var viewModel = new simpleViewModel();
    var result = viewModel.sum(2, 3);

    ok(result == 5, "Passed!");
})