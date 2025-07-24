// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var cards = document.querySelectorAll('#GalleryInventory > .card');
for (var i = 0; i < cards.length; i++) {
  var title = cards[i].querySelector(`#card-${i}-title`);
  title.textContent = `[JS] ${title.textContent}`;
  console.log(`Altered card ${i}`)
}


/*
var myCarousel = document.querySelector('#carousel')
var carousel = new bootstrap.Carousel(myCarousel, {
  interval: 2000,
  wrap: false
});
*/