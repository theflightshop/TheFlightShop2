
function openInstallationExamples() {
    document.getElementById('flightshop-installation-examples').style.display = 'block';
}

function closeInstallationExamples() {
    document.getElementById('flightshop-installation-examples').style.display = 'none';
}

var unlistedPartCount = 1;
function addUnlistedPart(productId, imgSrc) {
    var specialPartRow = document.createElement('tr');

    var partNumberCol = document.createElement('td');
    partNumberCol.classList.add('flightshop-parts-table-cell');

    var partNumberInput = document.createElement('input');
    partNumberInput.id = 'part-nr-input-' + unlistedPartCount;
    partNumberInput.placeholder = 'Part #';
    partNumberInput.style.maxWidth = '75%';
    partNumberInput.style.float = 'left';
    partNumberCol.appendChild(partNumberInput);
    partNumberCol.style.borderRight = 'none';

    var orCol = document.createElement('span');
    orCol.innerHTML = ' or ';
    orCol.style.width = '50px';
    orCol.style.marginLeft = '1em';
    partNumberCol.appendChild(orCol);

    var descriptionCol = document.createElement('td');
    descriptionCol.classList.add('flightshop-parts-table-cell');

    var descriptionInput = document.createElement('input');
    descriptionInput.id = 'description-input-' + unlistedPartCount;
    descriptionInput.placeholder = 'Description of part';
    descriptionCol.appendChild(descriptionInput);
    descriptionCol.style.borderLeft = 'none';

    var priceCol = document.createElement('td');
    priceCol.classList.add('flightshop-parts-table-cell');
    priceCol.style.fontStyle = 'italic';
    priceCol.innerHTML = '(quote)';

    var addToCartCol = document.createElement('td');
    addToCartCol.classList.add('flightshop-parts-table-cell');

    var addToCartInput = document.createElement('input');
    addToCartInput.id = 'item-quantity-' + unlistedPartCount;
    addToCartInput.classList.add('flightshop-part-add-to-cart-input');
    addToCartInput.value = '1';
    addToCartInput.type = 'number';
    addToCartInput.style.marginRight = '5px';
    addToCartCol.appendChild(addToCartInput);

    var addToCartButton = document.createElement('button');
    addToCartButton.id = 'button-add-' + unlistedPartCount;
    addToCartButton.type = 'button';
    addToCartButton.innerHTML = 'Add';
    var inputId = unlistedPartCount;
    addToCartButton.addEventListener('click', function () {
        addUnlistedToCart('' + inputId, productId, imgSrc)
    });
    addToCartCol.appendChild(addToCartButton);

    specialPartRow.appendChild(partNumberCol);
    specialPartRow.appendChild(descriptionCol);
    specialPartRow.appendChild(priceCol);
    specialPartRow.appendChild(addToCartCol);
    document.getElementById('flightshop-parts-table').appendChild(specialPartRow);

    unlistedPartCount++;
}

function addUnlistedToCart(inputId, productId, imgSrc) {
    var partNumber = document.getElementById('part-nr-input-' + inputId).value;
    var description = document.getElementById('description-input-' + inputId).value;
    var quantity = getQuantity('item-quantity-' + inputId);

    var validationAlert = document.getElementById('flightshop-invalid-unlisted-part-alert');
    var quantityAlert = document.getElementById('flightshop-invalid-part-quantity');
    var infoProvided = partNumber || description;

    if (infoProvided && quantity) {
        validationAlert.style.display = 'none';
        quantityAlert.style.display = 'none';
        requestAddToCart(productId, infoProvided, quantity, null, imgSrc);
    }
    else if (infoProvided) {
        quantityAlert.style.display = 'block';
    }
    else {
         validationAlert.style.display = 'block';
    }

    renderCartButton();
}

function addToCart(productId, partNumber, price, imgSrc) {
    var quantity = getQuantity('addToCartQuantity-' + partNumber);
    var quantityAlert = document.getElementById('flightshop-invalid-part-quantity');

    if (quantity) {
        quantityAlert.style.display = 'none';
        requestAddToCart(productId, partNumber, quantity, price, imgSrc);
    }
    else {
        quantityAlert.style.display = 'block';
    }

    renderCartButton();
}

function getQuantity(id) {
    var value = document.getElementById(id).value;
    var parsed = parseFloat(value);
    return parsed > 0 ? parsed : null;
}

function requestAddToCart(productId, codeOrDescription, quantity, price, imgSrc) {
    addItemToProductAddedDisplay(codeOrDescription, quantity, price);
    var cartValue = window.sessionStorage.getItem('cartItems');
    var cart = cartValue ? JSON.parse(cartValue) : [];
    cart.push({
        ProductId: productId,
        PartNumber: codeOrDescription,
        Quantity: quantity,
        ImageSrc: imgSrc
    });
    window.sessionStorage.setItem('cartItems', JSON.stringify(cart));
    showProductAddedDisplay(true);
}

function addItemToProductAddedDisplay(codeOrDescription, quantity, price) {
    var itemAdded = document.createElement('p');
    itemAdded.classList.add('flightshop-product-added-item-container');

    var itemDescription = document.createElement('em');
    itemDescription.classList.add('flightshop-product-added-item');
    var descriptionValue = codeOrDescription.slice(0, 15);
    if (codeOrDescription.length > 15) {
        descriptionValue += '...';
    }
    itemDescription.innerHTML = quantity + '&times;&nbsp;' + descriptionValue + '&nbsp;'
    itemAdded.appendChild(itemDescription);

    var itemPrice = document.createElement('span');
    itemPrice.classList.add('flightshop-product-added-price');
    var priceValue = parseFloat(price) || 0;
    if (priceValue) {
        var totalCost = priceValue * quantity;
        itemPrice.innerHTML = '$' + totalCost.toFixed(2);
    }
    else {
        itemPrice.innerHTML = '(quote)';
    }
    itemAdded.appendChild(itemPrice);

    var addedItemsDisplay = document.getElementById('flightshop-product-added-items');
    addedItemsDisplay.appendChild(itemAdded);
}

var currentAddedDisplayInterval = null;
var timeit = null;
function showProductAddedDisplay(fadeOut) {
    if (currentAddedDisplayInterval) {
        clearInterval(currentAddedDisplayInterval);
    }
    if (timeit) {
        clearTimeout(timeit);
    }

    var overlay = document.getElementById('flightshop-added-to-cart-overlay');
    var arrow = document.getElementById('flightshop-product-added-arrow');
    overlay.style.opacity = '1';
    overlay.style.display = 'block';
    arrow.style.opacity = '1';
    arrow.style.display = 'block';

    if (fadeOut) {
        timeit = setTimeout(function () {
            fadeProductAddedDisplay();
        }, 3000);
    }
}

function fadeProductAddedDisplay() {
    var overlay = document.getElementById('flightshop-added-to-cart-overlay');
    var arrow = document.getElementById('flightshop-product-added-arrow');
    currentAddedDisplayInterval = setInterval(function () {
        var opacity = overlay.style.opacity;
        if (opacity === '0') {
            hideProductAddedDisplay(overlay, arrow);
        }
        else {
            var opacityValue = (parseFloat(opacity) || '1') - 1/50;
            if (opacityValue <= 0) {
                opacityValue = 0;
                hideProductAddedDisplay(overlay, arrow);
            }
            overlay.style.opacity = opacityValue;
            arrow.style.opacity = opacityValue;
        }

    }, 1000 / 50);
}

function hideProductAddedDisplay(overlay, arrow) {
    if (currentAddedDisplayInterval) {
        clearInterval(currentAddedDisplayInterval);
        currentAddedDisplayInterval = null;
    }
    overlay.style.display = 'none';
    arrow.style.display = 'none';
    document.getElementById('flightshop-product-added-items').innerHTML = '';
}

function closeProductsAddedOverlay() {
    var overlay = document.getElementById('flightshop-product-added-arrow');
    var arrow = document.getElementById('flightshop-added-to-cart-overlay');
    hideProductAddedDisplay(overlay, arrow);
}

