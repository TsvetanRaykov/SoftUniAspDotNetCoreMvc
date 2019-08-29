function DeleteNewOrderConfirm() {
    window.bootbox.confirm({

        title: "Order remove confirmation!",
        message: "Are you sure?",
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

                window.location.href = window.location.pathname.replace("OrderNew", "OrderNewRemove");
            }
        }
    });
}