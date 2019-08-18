let selectPropertyItem = $("#vendor-product-detail-ddl");
let selectValueItem = $("#new-property-value");
let tableAddProductDetails = $("#table-add-product-details");
let rowTemplate = $("#table-add-product-details-row-template");


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

    let currentDetails = $("#product-details").val();
    let productDetails = JSON.parse(currentDetails);

    let property = selectPropertyItem.find("option:selected");

    let newRow = rowTemplate.clone();
    newRow.find("td:first-child div").text(property.text());
    newRow.find("td:last-child div").text(selectValueItem.val());
    newRow.removeClass("d-none");

    let newProp = {
        "commonDetailId": property.val(),
        "value": selectValueItem.val()
    }

    let add = true;
    for (let d of productDetails) {
        if (d.commonDetailId == newProp.commonDetailId && d.value == newProp.value) {
            add = false;
        }
    }

    if (add) {
        productDetails.push(newProp);
        $("#product-details").val(JSON.stringify(productDetails));
        let btn = $(newRow).find("button");
        $(btn).attr("data-id", newProp.commonDetailId);
        $(btn).attr("data-value", newProp.value);
        newRow.appendTo(tableAddProductDetails);
    }

}

function removeProductDetailsFromForm(e) {
    const currentDetails = $("#product-details").val();
    const productDetails = JSON.parse(currentDetails);

    const value = $(e).data("value");
    const id = $(e).data("id");

    for (let i = 0; i < productDetails.length; i++) {
        if (productDetails[i].commonDetailId == id && productDetails[i].value == value) {
            productDetails.splice(i, 1);
            $("#product-details").val(JSON.stringify(productDetails));
            $(e).parent().parent().remove();
            break;
        }
    }
}

function RemoveGalleryImage(e) {
    let img = $(e.target);
    window.bootbox.confirm({
        title: "Delete action confirmation!",
        message: `Are you sure to remove this image from product galery?`,
        className: "vxp-delete-confirmation-dialog",
        buttons: {
            confirm: {
                label: "Yes",
                className: "btn-success"
            },
            cancel: {
                label: "No",
                className: "btn-danger"
            }
        },
        swapButtonOrder: true,
        centerVertical: false,
        callback: function (result) {
            if (result) {
                let currentImages = $("#product-images").val();
                let productImages = JSON.parse(currentImages);
                for (let i = 0; i < productImages.length; i++) {
                    if (productImages[i].id === img.data("id")) {
                        productImages.splice(i, 1);
                        img.parent().parent().remove();
                    }
                }
                $("#product-images").val(JSON.stringify(productImages));
            }
        }
    });
}