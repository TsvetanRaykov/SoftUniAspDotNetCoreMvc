let selectPropertyItem = $("#vendor-product-detail-ddl");
let selectValueItem = $("#new-property-value");
let tableAddProductDetails = $("#table-add-product-details");
let rowTemplate = $("#table-add-product-details-row-template");
let productDetails = [];

selectPropertyItem.change(() => {
    addProductDetailValidate();
});

selectValueItem.change(() => {
    addProductDetailValidate();
});

function addProductDetailValidate() {
    
    let result = true;

    selectPropertyItem.removeClass("vxp-input-invalid");
    selectValueItem.removeClass("vxp-input-invalid");

    if (selectPropertyItem.find("option:selected").prop("disabled") === true) {
        selectPropertyItem.addClass("vxp-input-invalid");
        result = false;
    }

    if (!selectValueItem.val()) {
        selectValueItem.addClass("vxp-input-invalid");
        result = false;
    }

    return result;
}

function addProductDetailToTheForm() {

    if (!addProductDetailValidate()) { return; }

    let property = selectPropertyItem.find("option:selected");

    let newRow = rowTemplate.clone();
    newRow.find("td:first-child div").text(property.text());
    newRow.find("td:last-child div").text(selectValueItem.val());
    newRow.removeClass("d-none");

    //let detail = property.text().split(":");
    productDetails.push({
        "CommonDetailId": property.val(),
        "Value": selectValueItem.val()
    });

    $("#product-details").val(JSON.stringify(productDetails));

   // if (productDetails.length % 2 === 0) {
        newRow.find("div").addClass("bg-light");
    //}

    newRow.appendTo(tableAddProductDetails);
}
