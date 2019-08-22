
function openInstallationExamples() {
    document.getElementById('flightshop-installation-examples').style.display = 'block';
}

function closeInstallationExamples() {
    document.getElementById('flightshop-installation-examples').style.display = 'none';
}

function addSpecialPart() {
    var specialPartRow = document.createElement('tr');

    var partNumberCol = document.createElement('td');
    partNumberCol.classList.add('flightshop-parts-table-cell');

    var partNumberInput = document.createElement('input');
    partNumberInput.placeholder = 'Special part #';
    partNumberCol.appendChild(partNumberInput);

    var descriptionCol = document.createElement('td');
    descriptionCol.classList.add('flightshop-parts-table-cell');

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
    addToCartButton.type = 'button';
    addToCartButton.innerHTML = 'Add';
    addToCartCol.appendChild(addToCartButton);

    specialPartRow.appendChild(partNumberCol);
    specialPartRow.appendChild(descriptionCol);
    specialPartRow.appendChild(priceCol);
    specialPartRow.appendChild(addToCartCol);
    document.getElementById('flightshop-parts-table').appendChild(specialPartRow);
}