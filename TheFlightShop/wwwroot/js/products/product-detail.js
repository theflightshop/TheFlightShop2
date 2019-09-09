
function openInstallationExamples() {
    document.getElementById('flightshop-installation-examples').style.display = 'block';
}

function closeInstallationExamples() {
    document.getElementById('flightshop-installation-examples').style.display = 'none';
}

var unlistedPartCount = 1;
function addUnlistedPart() {
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
        addUnlistedToCart('' + inputId)
    });
    addToCartCol.appendChild(addToCartButton);

    specialPartRow.appendChild(partNumberCol);
    specialPartRow.appendChild(descriptionCol);
    specialPartRow.appendChild(priceCol);
    specialPartRow.appendChild(addToCartCol);
    document.getElementById('flightshop-parts-table').appendChild(specialPartRow);

    unlistedPartCount++;
}

function addUnlistedToCart(inputId) {
    var partNumber = document.getElementById('part-nr-input-' + inputId).value;
    var description = document.getElementById('description-input-' + inputId).value;
    var quantity = getQuantity('item-quantity-' + inputId);

    var validationAlert = document.getElementById('flightshop-invalid-unlisted-part-alert');
    var quantityAlert = document.getElementById('flightshop-invalid-part-quantity');
    var infoProvided = partNumber || description;

    if (infoProvided && quantity) {
        validationAlert.style.display = 'none';
        quantityAlert.style.display = 'none';
        requestAddToCart(infoProvided, quantity, null);
    }
    else if (infoProvided) {
        quantityAlert.style.display = 'block';
    }
    else {
         validationAlert.style.display = 'block';
    }
}

function addToCart(partNumber, price) {
    var quantity = getQuantity('addToCartQuantity-' + partNumber);
    var quantityAlert = document.getElementById('flightshop-invalid-part-quantity');

    if (quantity) {
        quantityAlert.style.display = 'none';
        requestAddToCart(partNumber, quantity, price);
    }
    else {
        quantityAlert.style.display = 'block';
    }
}

function getQuantity(id) {
    var value = document.getElementById(id).value;
    var parsed = parseFloat(value);
    return parsed > 0 ? parsed : null;
}

function requestAddToCart(codeOrDescription, quantity, price) {
    addItemToProductAddedDisplay(codeOrDescription, quantity, price);
}

function addItemToProductAddedDisplay(codeOrDescription, quantity, price) {
    var itemAdded = document.createElement('p');
    itemAdded.classList.add('flightshop-product-added-item-container');

    var itemDescription = document.createElement('em');
    itemDescription.classList.add('flightshop-product-added-item');
    itemDescription.innerHTML = quantity + '&times;&nbsp;' + codeOrDescription.slice(0, 14) + '&nbsp;'
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

    document.getElementById('flightshop-product-added-items').appendChild(itemAdded);
}

function closeProductsAddedOverlay() {
    document.getElementById('flightshop-added-to-cart-overlay').style.display = 'none';
    document.getElementById('flightshop-product-added-items').innerHTML = '';
}

