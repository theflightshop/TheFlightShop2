
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
        addToCart('' + inputId)
    });
    addToCartCol.appendChild(addToCartButton);

    specialPartRow.appendChild(partNumberCol);
    specialPartRow.appendChild(descriptionCol);
    specialPartRow.appendChild(priceCol);
    specialPartRow.appendChild(addToCartCol);
    document.getElementById('flightshop-parts-table').appendChild(specialPartRow);

    unlistedPartCount++;
}

function addToCart(inputId) {
    var partNumber = document.getElementById('part-nr-input-' + inputId).value;
    var description = document.getElementById('description-input-' + inputId).value;
    var infoProvided = partNumber || description;

    if (infoProvided) {
        console.log('add to cart', partNumber, description);
    }

    var validationAlert = document.getElementById('flightshop-invalid-unlisted-part-alert');
    validationAlert.style.display = infoProvided ? 'none' : 'block';
}