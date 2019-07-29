class DistributorEditForm {

    constructor(distributorId) {

        this.apiBaseUrl = "/api/Distributors/";
        this.distributorId = distributorId;
        this.loader = $("#modal-loader");
        this.form = $("#modalDistributor");

        this.submitButton = $("#btnDistributorFormSubmit");
        this.deleteButton = $("#btnDistributorFormDelete");
        this.errorPaceholder = $("#modalDistributor .vxp-validation-errors-placeholder");

        this.customerId = this.form.find('input[name="customer-id"]').val();

        this.inputFields = {

            distName: $("#dist-name"),
            distEmail: $("#dist-email"),
            distContactAddress: $("#dist-contact-address"),
            distContactEmail: $("#dist-contact-email"),
            distContactPhone: $("#dist-contact-phone"),
            distCompanyName: $("#dist-company-name"),
            distCompanyBin: $("#dist-company-bin"),
            distBankName: $("#dist-bank-name"),
            distBankIban: $("#dist-bank-iban"),
            distBankBic: $("#dist-bank-bic"),
            distBankSwift: $("#dist-bank-swift")

        }

        this.deleteButton.off("click").on("click", () => this.confirmDelete());

        this.setMode(distributorId ? "update" : "create");

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
    }

    setMode(mode) {

        this.reset();

        switch (mode) {
            case "create":
                this.submitButton.val("Create");
                this.method = "POST";
                this.deleteButton.hide();
                this.submitButton.show();
                this.Run = () => this.load(`${this.apiBaseUrl}GetAvailable/${this.customerId}`);
                break;
            default: // update
                this.submitButton.val("Update");
                this.method = "PUT";
                this.submitButton.hide();
                this.deleteButton.show();
                this.Run = () => this.load(this.apiBaseUrl + this.distributorId);
                break;
        }
        this.mode = mode;
    }

    load(url) {

        $.ajax({
            url: url,
            type: "GET",
            beforeSend: () => this.loader.modal('show'),
            success: (dataArray) => {
                if (this.mode !== "create") {
                    let data = dataArray[0];
                    this.inputFields.distName.text(`${data.firstName} ${data.lastName}`);
                    this.inputFields.distEmail.text(data.email);
                    this.inputFields.distContactAddress.text(
                        `${data.contactAddress.addressLocation}, ${data.contactAddress.city}, ${data.contactAddress
                            .countryName}`);
                    this.inputFields.distContactEmail.text(data.contactAddress.email);
                    this.inputFields.distContactPhone.text(data.contactAddress.phone);
                    this.inputFields.distCompanyName.text(data.company.name);
                    this.inputFields.distCompanyBin.text(data.company.businessNumber);
                    this.inputFields.distBankName.text(data.bankAccount.bankName);
                    this.inputFields.distBankIban.text(data.bankAccount.accountNumber);
                    this.inputFields.distBankBic.text(data.bankAccount.bicCode);
                    this.inputFields.distBankSwift.text(data.bankAccount.swiftCode);
                }
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

    clearErrors() {
        const inputFields = this.inputFields;
        for (let key in inputFields) {
            if (inputFields.hasOwnProperty(key)) {
                inputFields[key].removeClass("vxp-input-error");
            }
        }

        this.errorPaceholder.text("");
    }

    reset() {
        this.clearErrors();
        const inputFields = this.inputFields;
        for (let key in inputFields) {
            if (inputFields.hasOwnProperty(key)) {
                inputFields[key].text("");
            }
        }
    }

    submit() {

        this.form.validate(); // method from jquery.validate
        let that = this;

        $.ajax({
            url: this.apiBaseUrl,
            type: this.method,
            contentType: 'application/json',
            data: JSON.stringify({
                //id: this.distributorId,
                //ownerId: this.ownerId,
                //bankName: this.inputFields.bankName.val(),
                //accountNumber: this.inputFields.accountNumber.val(),
                //bicCode: this.inputFields.bicCode.val(),
                //swiftCode: this.inputFields.swiftCode.val()
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

    confirmDelete() {
        const that = this;
        window.bootbox.confirm({

            title: "Remove distributor from the list!",
            message: "Are you sure to drop the connection to this distributor?",
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
                if (result) { that.delete(); }
            }
        });
    }

    delete() {
        $.ajax({
            url: this.apiBaseUrl + this.distributorId,
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
