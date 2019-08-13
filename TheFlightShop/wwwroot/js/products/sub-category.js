function displayRFQ(productCode) {
    document.getElementById('flightshop-subcategory-content').style.display = 'none';
    document.getElementById('flightshop-active-subcategory-breadcrumb').style.display = 'none';
    document.getElementById('flightshop-return-to-subcategory-breadcrumb').style.display = 'initial';

    var productRfqBreadcrumb = document.getElementById('flightshop-product-rfq-breadcrumb');
    productRfqBreadcrumb.innerHTML = productCode + ' - RFQ';
    productRfqBreadcrumb.style.display = 'initial';

    document.getElementById('flightshop-product-rfq-content').style.display = 'block';
}

function hideRFQ() {
    document.getElementById('flightshop-product-rfq-content').style.display = 'none';
    document.getElementById('flightshop-product-rfq-breadcrumb').style.display = 'none';
    document.getElementById('flightshop-return-to-subcategory-breadcrumb').style.display = 'none';
    document.getElementById('flightshop-active-subcategory-breadcrumb').style.display = 'initial';
    document.getElementById('flightshop-subcategory-content').style.display = 'block';
}