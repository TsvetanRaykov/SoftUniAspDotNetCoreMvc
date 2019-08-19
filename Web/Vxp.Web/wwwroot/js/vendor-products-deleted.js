$('#table-deleted-products').DataTable({
    "lengthChange": false
});

function confirmProductDeletion(event) {
    let productName = $(event.currentTarget).data("name");
    event.preventDefault();

    window.bootbox.confirm({
        title: "Delete action confirmation!",
        message: `Are you sure to delete ${productName} from the system?<br/>It cannot be undone!`,
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
            if (result) { $(event.target).closest("form").submit(); }
        }
    });
}