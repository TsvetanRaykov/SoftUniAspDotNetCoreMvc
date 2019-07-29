function BankAccountEditForm(accountId) {


    this.accountId = accountId;
    this.loader = $("#modal-loader");
    this.form = $("#modalBankAccount");

    this.submitButton = $("#btnBankAccountSubmit");
    this.deleteButton = $("#btnBankAccountDelete");
    this.errorPaceholder = $("#modalBankAccount .vxp-validation-errors-placeholder");

    this.ownerId = this.form.find('input[name="owner-id"]').val();

    this.inputFields = {
        bankName: this.form.find('input[name="bank-name"]'),
        accountNumber: this.form.find('input[name="account-number"]'),
        bicCode: this.form.find('input[name="bic-code"]'),
        swiftCode: this.form.find('input[name="swift-code"]')
    }

    this.deleteButton.off("click").on("click", () => this.confirmDelete());


    this.clearErrors = () => {
        const inputFields = this.inputFields;
        for (let key in inputFields) {
            if (inputFields.hasOwnProperty(key)) {
                inputFields[key].removeClass("vxp-input-error");
            }
        }

        this.errorPaceholder.text("");
    }

    this.load = () => {

        $.ajax({
            url: "/api/BankAccounts/" + this.accountId,
            type: "GET",
            beforeSend: () => this.loader.modal('show'),
            success: (data) => {
                this.inputFields.bankName.val(data.bankName);
                this.inputFields.accountNumber.val(data.accountNumber);
                this.inputFields.bicCode.val(data.bicCode);
                this.inputFields.swiftCode.val(data.swiftCode);
                this.form.modal('show');
            },
            error: (data) => {
                console.error(data);
            },
            complete: () => {
                this.loader.modal('hide');
            }
        });
    }

    this.reset = () => {
        this.clearErrors();
        const inputFields = this.inputFields;
        for (let key in inputFields) {
            if (inputFields.hasOwnProperty(key)) {
                inputFields[key].val("");

            }
        }
    }

    this.setMode = (mode) => {

        this.reset();

        switch (mode) {
            case "create":
                this.submitButton.val("Create");
                this.method = "POST";
                this.Run = () => this.form.modal('show');
                this.deleteButton.hide();
                break;
            default: // update
                this.submitButton.val("Update");
                this.method = "PUT";
                this.deleteButton.show();
                this.Run = () => this.load();
                break;
        }
    }

    this.setMode(accountId ? "update" : "create");

    const that = this;
    this.form.off("submit").on("submit", function (e) {
        e.preventDefault();
        that.submit();
        return false;
    });

    for (let key in that.inputFields) {
        if (that.inputFields.hasOwnProperty(key)) {
            that.inputFields[key].on("change keydown paste input",
                function () {
                    that.clearErrors();
                });
        }
    }

    this.Run();

   

    this.submit = () => {

        this.form.validate(); // method from jquery.validate

        let that = this;

        $.ajax({
            url: "/api/BankAccounts/",
            type: this.method,
            contentType: 'application/json',
            data: JSON.stringify({
                id: this.accountId,
                ownerId: this.ownerId,
                bankName: this.inputFields.bankName.val(),
                accountNumber: this.inputFields.accountNumber.val(),
                bicCode: this.inputFields.bicCode.val(),
                swiftCode: this.inputFields.swiftCode.val()
            }),
            success: function () {
                window.location.reload();
            },
            error: function (data) {
                let response = JSON.parse(data.responseText);
                let errors = [];
                for (let key in response.errors) {
                    if (response.errors.hasOwnProperty(key)) {

                        errors.push(response.errors[key]);

                        key = key.replace(/([a-zA-Z])(?=[A-Z])/g, '$1-').toLowerCase();

                        let field = that.form.find(`input[name="${key}"]`);
                        field.addClass("vxp-input-error");

                    }
                }

                that.errorPaceholder.text(errors.join("<br/>"));
            },
            complete: function () {
                // ignore
            }
        });
    }

    this.confirmDelete = () => {
        const that = this;
        window.bootbox.confirm({

            title: "Bank account delete confirmation!",
            message: "Are you sure to delete this bank account?",
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
                if (result) { that.deleteAccount(); }
            }
        });
    }

    this.deleteAccount = () => {
        $.ajax({
            url: "/api/BankAccounts/" + this.accountId,
            type: "DELETE",
            success: () => {
                window.location.reload();
            },
            error: (data) => {
                console.error(data);
            },
            complete: () => {
                // ignore
            }
        });
    }
}
