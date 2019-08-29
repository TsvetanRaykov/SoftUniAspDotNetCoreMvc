function deleteProject(e) {

    e.preventDefault();
    let form = e.currentTarget;
    let projectName = e.currentTarget.dataset['name'];
    window.bootbox.confirm({
        title: "Project delete confirmation!",
        message: `Are you sure to delete ${projectName} project?`,
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
                form.submit();
            }
        }
    });
}
