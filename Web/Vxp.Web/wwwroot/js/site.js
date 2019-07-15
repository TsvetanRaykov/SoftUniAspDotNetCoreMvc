// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).on('click', '[data-toggle="lightbox"]', function (event) {
    event.preventDefault();
    $(this).ekkoLightbox();
});

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