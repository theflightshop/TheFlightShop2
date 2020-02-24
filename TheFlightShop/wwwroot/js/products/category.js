function selectedCategoryChange(selectedValue) {
    var productLinkContainers = document.getElementsByClassName('flightshop-product-link-container');
    for (var i = 0; i < productLinkContainers.length; i++) {
        var container = productLinkContainers[i];
        if (selectedValue === 'all') {
            container.style.display = 'inline-block';
        }
        else {
            var subCategory = container.getAttribute('data-subcategory');
            var isDisplayed = subCategory && subCategory.trim() === selectedValue;
            container.style.display = isDisplayed ? 'inline-block' : 'none';
        }
    }
}