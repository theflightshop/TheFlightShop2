// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//var burgerFlipped = false;
//function hamburgerClick() {
//    burgerFlipped = !burgerFlipped;
//    document.getElementById('flightshop-expand-menu').style.display = burgerFlipped ? 'block' : 'none';
//    setTimeout(function () {
//        document.getElementById('flightshop-expand-menu').click();
//    }, 1000);
//}
function myFunction() {
    document.getElementById("myDropdown").classList.toggle("show");
}
window.onclick = function (event) {
    if (!event.target.matches('.dropbtn')) {
        var dropdowns = document.getElementsByClassName("dropdown-content");
        var i;
        for (i = 0; i < dropdowns.length; i++) {
            var openDropdown = dropdowns[i];
            if (openDropdown.classList.contains('show')) {
                openDropdown.classList.remove('show');
            }
        }
    }
}

function getCart() {
    var cartText = window.sessionStorage.getItem('cartItems');
    return cartText ? JSON.parse(cartText) : [];
}