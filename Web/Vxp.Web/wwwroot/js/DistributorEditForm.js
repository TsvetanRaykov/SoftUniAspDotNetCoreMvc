class DistributorEditForm {

    constructor(distributorId) {

        this.distributor = {};
        this.apiBaseUrl = "/api/Distributors/";
        this.distributorId = distributorId;
        this.loader = $("#modal-loader");
        this.form = $("#modalDistributor");

        this.submitButton = $("#btnDistributorFormSubmit");
        this.deleteButton = $("#btnDistributorFormDelete");
        this.distSelector = $("#dist-form-selector");

        this.errorPaceholder = $("#modalDistributor .vxp-validation-errors-placeholder");

        this.customerId = this.form.find('input[name="customer-id"]').val();

        this.fields = {
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

        this.distSelector.off("change").on("change", (e) => this.selectDistributor(e));
        this.deleteButton.off("click").on("click", () => this.confirmDelete());

        this.setMode(distributorId ? "update" : "create");
        const that = this;
        this.form.off("submit").on("submit", function (e) {
            e.preventDefault();
            that.submit();
            return false;
        });

        for (let key in that.fields) {
            if (that.fields.hasOwnProperty(key)) {
                that.fields[key].on("change keydown paste input",
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
                this.submitButton.val("Connect");
                this.method = "POST";
                this.deleteButton.hide();
                this.distSelector.show()
                this.submitButton.show();
                this.Run = () => this.load(`${this.apiBaseUrl}GetAvailable/${this.customerId}`);
                break;
            default: // update
                this.submitButton.val("Update");
                this.method = "PUT";
                this.submitButton.hide();
                this.deleteButton.show();
                this.distSelector.hide()
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
                    this.populate(data);
                } else {

                    for (let dist of dataArray) {
                        this.distributor[dist.email] = dist;
                        this.distSelector.append(`<option value="${dist.email}">${dist.company.name}</option>`)
                    }
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
        const inputFields = this.fields;
        for (let key in inputFields) {
            if (inputFields.hasOwnProperty(key)) {
                inputFields[key].removeClass("vxp-input-error");
            }
        }

        this.errorPaceholder.text("");
    }

    populate(data) {

        this.fields.distName.text(`${data.firstName} ${data.lastName}`);
        this.fields.distEmail.text(data.email);
        this.fields.distContactAddress.text(
            `${data.contactAddress.addressLocation}, ${data.contactAddress.city}, ${data.contactAddress
                .countryName}`);
        this.fields.distContactEmail.text(data.contactAddress.email);
        this.fields.distContactPhone.text(data.contactAddress.phone);
        this.fields.distCompanyName.text(data.company.name);
        this.fields.distCompanyBin.text(data.company.businessNumber);
        this.fields.distBankName.text(data.bankAccount.bankName);
        this.fields.distBankIban.text(data.bankAccount.accountNumber);
        this.fields.distBankBic.text(data.bankAccount.bicCode);
        this.fields.distBankSwift.text(data.bankAccount.swiftCode);
        this.distributorId = data.email;
    }

    reset() {
        this.clearErrors();
        const inputFields = this.fields;
        for (let key in inputFields) {
            if (inputFields.hasOwnProperty(key)) {
                inputFields[key].text("");
            }
        }

        this.distSelector.find('option').each(function (index, element) {
            if (index > 0) { $(element).remove(); }
        });

        this.submitButton.attr('disabled', 'disabled');
    }

    submit() {

        this.form.validate(); // method from jquery.validate
        let that = this;

        $.ajax({
            url: this.apiBaseUrl,
            type: this.method,
            contentType: 'application/json',
            data: JSON.stringify({
                CustomerEmail: this.customerId,
                DistributorEmail: this.distributorId
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

                that.errorPaceholder.html(errors.join("<br/>"));
            },
            complete: function () {
                // ignore
            }
        });
    }

    selectDistributor(e) {
        let distEmail = e.currentTarget.options[e.currentTarget.selectedIndex].value;
        this.populate(this.distributor[distEmail]);
        this.submitButton.removeAttr('disabled');
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
            url: this.apiBaseUrl + "disconnect/",
            type: "POST",
            contentType: 'application/json',
            data: JSON.stringify({
                CustomerEmail: this.customerId,
                DistributorEmail: this.distributorId
            }),
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
