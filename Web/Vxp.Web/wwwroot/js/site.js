
let guestImage = $(".hero-wrap.js-full-height");

if (guestImage.length === 0) {


    let navbar = $('.ftco_navbar');
    let sd = $('.js-scroll-wrap');

    if (!navbar.hasClass('scrolled')) {
        navbar.addClass('scrolled');
    }

    if (!navbar.hasClass('awake')) {
        navbar.addClass('awake');
    }

    if (sd.length > 0) {
        sd.addClass('sleep');
    }

    navbar.removeClass('ftco_navbar');
}

function SetUnderlinedNavLinks(navbar, v) {

    var navItems = $(navbar).find("a.nav-link");

    if (v < 150) {
        for (var i = 0; i < navItems.length; i++) {
            $(navItems[i]).removeClass("vxp-underline-primary");
            if (!$(navItems[i]).hasClass("vxp-underline-light")) {
                $(navItems[i]).addClass("vxp-underline-light");
            }
        }
    } else {
        for (var i = 0; i < navItems.length; i++) {
            $(navItems[i]).removeClass("vxp-underline-light");
            if (!$(navItems[i]).hasClass("vxp-underline-primary")) {
                $(navItems[i]).addClass("vxp-underline-primary");
            }
        }
    }
}

function SendVerificationEmail(userId) {
    let token = $('input[name="__RequestVerificationToken"]').val();
    $.ajax({
        url: "/Users/SendVerificationEmailAsync",
        type: "POST",
        headers: {
            'RequestVerificationToken': token
        },
        data: {
            userId: userId
        },
        success: () => {
            window.location = window.location.href;
        },
        error: (data) => {
            console.error(data);
        },
        complete: () => {
            // ignore
        }
    });
}

function addToOrder(productId) {
    let token = $('input[name="__RequestVerificationToken"]').val();
    let badge = $('.vxp-card-order-preview .badge');
    $.ajax({
        url: "/Orders/AddProductToOrder",
        type: "POST",
        headers: {
            'RequestVerificationToken': token
        },
        data: {
            productId: productId
        },
        success: (count) => {
            badge.text(count);
        },
        error: (data) => {
            console.error(data);
        },
        complete: () => {
            // ignore
        }
    });
}

$('.vxp-data-table').DataTable({
    "lengthChange": false
});