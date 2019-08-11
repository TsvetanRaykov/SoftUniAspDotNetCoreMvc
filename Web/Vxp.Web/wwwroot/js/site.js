

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
