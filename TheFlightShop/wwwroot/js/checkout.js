function isPaymentType(paymentType) {
    return document.getElementById('flightshop-checkout-payment-type').value.toString() === paymentType.toString();
}

function paymentTypeChanged(creditCardPaymentType, onAccountPaymentType) {
    document.getElementById('flightshop-credit-card-container').style.display = isPaymentType(creditCardPaymentType) ? 'block' : 'none'; 
    document.getElementById('flightshop-po-number-container').style.display = isPaymentType(onAccountPaymentType) ? 'block' : 'none';
    document.getElementById('flightshop-checkout-submit-btn').style.display = 'inline-block';
}

function shippingTypeChanged(otherShipType) {
    var isCustomShipType = document.getElementById('flightshop-shipping-type').value === otherShipType;
    var shippingTypeTextDisplay = isCustomShipType ? 'inline-block' : 'none';
    var customShipType = document.getElementById('flightshop-customer-custom-shipping-type');
    customShipType.style.display = shippingTypeTextDisplay;
    if (isCustomShipType) {
        customShipType.setAttribute('required', true);
        customShipType.classList.add('mandatory-checkout-field');
    } else {
        customShipType.removeAttribute('required');
        customShipType.classList.remove('mandatory-checkout-field');
    }
}

function countryChanged(formType) {
    var id = 'flightshop-customer-country';
    var stateId = 'flightshop-customer-state';
    var altStateId = 'flightshop-customer-state-custom';
    var labelId = 'customer-state-label';
    if (formType === 'billing') {
        id += '-billing';
        stateId += '-billing';
        altStateId += '-billing';
        labelId += '-billing';
    }

    var state = document.getElementById(stateId);
    var altState = document.getElementById(altStateId);
    var label = document.getElementById(labelId);
    var isMerica = document.getElementById(id).value === 'US';
    if (isMerica) {
        label.innerHTML = 'State/Province/Region';
        state.setAttribute('required', true);
        state.classList.add('mandatory-checkout-field');
    } else {
        label.innerHTML = 'State/Province/Region (Optional)';
        state.removeAttribute('required');
        state.classList.remove('mandatory-checkout-field');
        state.parentElement.classList.remove('has-error');
    }

    state.style.display = isMerica ? 'inline-block' : 'none';
    altState.style.display = isMerica ? 'none' : 'inline-block';
}

function useShippingAddrForBilling() {
    return document.getElementById('flightshop-billing-use-shipping').checked || false;
}

function billingUseShippingChecked() {
    var checked = useShippingAddrForBilling();
    var billingAddrDisplay = checked ? 'none' : 'block';
    var addressRows = document.getElementsByClassName('billing-addr-row');
    [].forEach.call(addressRows, function (row) {
        row.style.display = billingAddrDisplay;
    });
    var isBillingAddrRequired = !checked;
    var formControls = document.getElementsByClassName('billing-addr-form-control');
    [].forEach.call(formControls, function (control) {
        if (isBillingAddrRequired) {
            control.setAttribute('required', true);
            control.classList.add('mandatory-checkout-field');
        } else {
            control.removeAttribute('required');
            control.classList.remove('mandatory-checkout-field');
        }
    });
}

function setAddresses(clientOrder, useShippingAddressForBilling) {
    clientOrder.Address1 = document.getElementById('flightshop-customer-addr-1').value;
    clientOrder.Address2 = document.getElementById('flightshop-customer-addr-2').value || null;
    clientOrder.CompanyName = document.getElementById('flightshop-customer-company-name').value || null;
    clientOrder.AttentionTo = document.getElementById('flightshop-customer-attention-to').value || null;
    clientOrder.City = document.getElementById('flightshop-customer-city').value;
    clientOrder.CountryCode = document.getElementById('flightshop-customer-country').value;
    clientOrder.State = clientOrder.CountryCode === 'US' ? document.getElementById('flightshop-customer-state').value : document.getElementById('flightshop-customer-state-custom').value;
    clientOrder.Zip = document.getElementById('flightshop-customer-zipcode').value;

    if (!useShippingAddressForBilling) {
        clientOrder.BillingAddress1 = document.getElementById('flightshop-customer-addr-1-billing').value;
        clientOrder.BillingAddress2 = document.getElementById('flightshop-customer-addr-2-billing').value || null;
        clientOrder.BillingCity = document.getElementById('flightshop-customer-city-billing').value;
        clientOrder.BillingCountryCode = document.getElementById('flightshop-customer-country-billing').value;
        clientOrder.BillingState = clientOrder.BillingCountryCode === 'US' ? document.getElementById('flightshop-customer-state-billing').value : document.getElementById('flightshop-customer-state-custom-billing').value;
        clientOrder.BillingZip = document.getElementById('flightshop-customer-zipcode-billing').value;
    }
}

function limitCreditCardExpLength(input) {
    if (input.value && input.value.length > 4) {
        input.value = input.value.slice(0, 4);
    }
}

function saveInputValueToStorage(id, optionalStorageKey) {
    var value = document.getElementById(id).value;
    var storageKey = optionalStorageKey || id;
    window.sessionStorage.setItem(storageKey, value);
}

function saveInputValuesToStorage() {
    saveInputValueToStorage('flightshop-customer-first-name');
    saveInputValueToStorage('flightshop-customer-last-name');
    saveInputValueToStorage('flightshop-customer-phone');
    saveInputValueToStorage('flightshop-customer-email');
    saveInputValueToStorage('flightshop-shipping-type');
    saveInputValueToStorage('flightshop-customer-custom-shipping-type');
    saveInputValueToStorage('flightshop-po-number');
    saveInputValueToStorage('flightshop-customer-company-name');
    saveInputValueToStorage('flightshop-customer-attention-to');
    saveInputValueToStorage('flightshop-customer-addr-1');
    saveInputValueToStorage('flightshop-customer-addr-2');
    saveInputValueToStorage('flightshop-customer-city');
    saveInputValueToStorage('flightshop-customer-zipcode');
    saveInputValueToStorage('flightshop-customer-country');
    saveInputValueToStorage('flightshop-cust-notes');
    if (useShippingAddrForBilling()) {
        saveInputValueToStorage('flightshop-customer-addr-1', 'flightshop-customer-addr-1-billing');
        saveInputValueToStorage('flightshop-customer-addr-2', 'flightshop-customer-addr-2-billing');
        saveInputValueToStorage('flightshop-customer-city', 'flightshop-customer-city-billing');
        saveInputValueToStorage('flightshop-customer-zipcode', 'flightshop-customer-zipcode-billing');
        saveInputValueToStorage('flightshop-customer-country', 'flightshop-customer-country-billing');
    } else {
        saveInputValueToStorage('flightshop-customer-addr-1-billing');
        saveInputValueToStorage('flightshop-customer-addr-2-billing');
        saveInputValueToStorage('flightshop-customer-city-billing');
        saveInputValueToStorage('flightshop-customer-zipcode-billing');
        saveInputValueToStorage('flightshop-customer-country-billing');
    }

    var stateValue = document.getElementById('flightshop-customer-state').value;
    var stateCustomValue = document.getElementById('flightshop-customer-state-custom').value;
    var billingStateValue = '';
    var billingStateCustomValue = ''
    if (useShippingAddrForBilling()) {
        billingStateValue = stateValue;
        billingStateCustomValue = stateCustomValue;
    } else {
        billingStateValue = document.getElementById('flightshop-customer-state-billing').value;
        billingStateCustomValue = document.getElementById('flightshop-customer-state-custom-billing').value;
    }
    window.sessionStorage.setItem('flightshop-customer-state', stateValue);
    window.sessionStorage.setItem('flightshop-customer-state-custom', stateCustomValue);
    window.sessionStorage.setItem('flightshop-customer-state-billing', billingStateValue);
    window.sessionStorage.setItem('flightshop-customer-state-custom-billing', billingStateCustomValue);
}

function submitCustomerInfo(customerInfoUrl, errorRedirectUrl, customShipType, formUrlReturned) {
    var shippingType = parseInt(document.getElementById('flightshop-shipping-type').value);
    var clientOrder = {
        FirstName: document.getElementById('flightshop-customer-first-name').value,
        LastName: document.getElementById('flightshop-customer-last-name').value,
        Email: document.getElementById('flightshop-customer-email').value,
        Phone: document.getElementById('flightshop-customer-phone').value,

        ShippingType: shippingType,
        PaymentType: parseInt(document.getElementById('flightshop-checkout-payment-type').value),
        CustomShippingType: shippingType === customShipType ? document.getElementById('flightshop-customer-custom-shipping-type').value || null : null,
        PurchaseOrderNumber: document.getElementById('flightshop-po-number').value || null,
        Notes: document.getElementById('flightshop-cust-notes').value || null
    };
    var useShippingAddressForBilling = useShippingAddrForBilling();
    setAddresses(clientOrder, useShippingAddressForBilling);

    var orderLines = [];
    var cart = JSON.parse(window.sessionStorage.getItem('cartItems'));
    for (var i = 0; i < cart.length; i++) {
        var item = cart[i];
        var orderLine = {
            PartNumber: item.PartNumber,
            ProductId: item.ProductId || null,
            Description: item.Description || null,
            Quantity: parseInt(item.Quantity) || 0,
            Price: parseFloat(item.Price) || null
        };
        orderLines.push(orderLine);
    }
    clientOrder.OrderLines = orderLines;

    saveInputValuesToStorage();

    $.ajax({
        type: 'POST',
        url: customerInfoUrl,
        data: clientOrder
    }).done(function (response) {
        if (response.confirmationNumber) {
            clearSessionStorage();
            clearCart();
            document.getElementById('flightshop-alternate-confirmation-number').innerHTML = response.confirmationNumber;
            document.getElementById('flightshop-checkout-section-processing').style.display = 'none';
            document.getElementById('flightshop-alternate-checkout-confirmation').style.display = 'block';
            document.getElementById('flightshop-checkout-glyphicon').classList.add('glyphicon-ok');
            document.getElementById('checkout-step-payment').classList.remove('active');
            document.getElementById('checkout-step-confirmation').classList.add('active');
            document.getElementById('flightshop-checkout-title').style.display = 'block';
            document.getElementById('flightshop-checkout-steps-header').style.display = 'block';
        } else {
            formUrlReturned(response);
        }
    }).fail(function () {
        location.href = errorRedirectUrl;
    });
}

function submitOrder(customerInfoUrl, errorRedirectUrl, customShipType) {
    var checkoutSections = document.getElementsByClassName('flightshop-checkout-section');
    for (var i = 0; i < checkoutSections.length; i++) {
        checkoutSections[i].style.display = 'none';
    }
    document.getElementById('flightshop-checkout-steps-header').style.display = 'none';
    document.getElementById('flightshop-checkout-title').style.display = 'none';
    document.getElementById('flightshop-checkout-section-processing').style.display = 'block';
    submitCustomerInfo(customerInfoUrl, errorRedirectUrl, customShipType, function (formUrl) {
        var cardForm = document.getElementById('flightshop-checkout-form');
        cardForm.action = formUrl;
        cardForm.submit();
    });    
}

function validatePoNumber() {
    var poNumber = document.getElementById('flightshop-po-number');
    var validationMsg = document.getElementById('billing-po-number-validation-msg');
    var hasPoNumber = poNumber.value && poNumber.value.length;
    if (hasPoNumber) {
        poNumber.classList.remove('invalid-input');
        validationMsg.style.display = 'none';
    } else {
        poNumber.classList.add('invalid-input');
        validationMsg.style.display = 'block';
    }
    return hasPoNumber;
}

function validateCardInfo() {
    var cardNumber = document.getElementById('billing-cc-number');
    var validationMsg = null;
    if (cardNumber.value) {
        var cardExp = document.getElementById('billing-cc-exp');
        if (!cardExp.value) {
            validationMsg = 'Please fill out this field.';
        } else if (cardExp.value.length < 4) {
            validationMsg = 'Please enter valid date in MMYY format.';
        } else {
            var month = parseInt(cardExp.value.slice(0, 2));
            if (isNaN(month) || month > 12) {
                validationMsg = 'Please enter month between 01 to 12.';
            }
        }

        if (validationMsg) {
            var expErrorMsg = document.getElementById('billing-cc-exp-validation-msg');
            expErrorMsg.innerHTML = validationMsg;
            cardExp.classList.add('invalid-input');
            expErrorMsg.style.display = 'block';
        } else {
            cardExp.classList.remove('invalid-input');
            document.getElementById('billing-cc-exp-validation-msg').style.display = 'none';

            var cvv = document.getElementById('billing-cvv');
            if (!cvv.value || cvv.value.length < 3 || cvv.value.length > 4) {
                var cvvErrorMessage = document.getElementById('billing-cvv-validation-msg');
                cvvErrorMessage.innerHTML = validationMsg = 'Please enter valid CVV value.';
                cvv.classList.add('invalid-input');
                cvvErrorMessage.style.display = 'block';
            } else {
                cvv.classList.remove('invalid-input');
                document.getElementById('billing-cvv-validation-msg').style.display = 'none';
            }
        }

        cardNumber.classList.remove('invalid-input');
        document.getElementById('billing-cc-number-validation-msg').style.display = 'none';
    } else {
        var cardNumErrorMsg = document.getElementById('billing-cc-number-validation-msg');
        cardNumErrorMsg.innerHTML = validationMsg = 'Please fill out this field';
        cardNumber.classList.add('invalid-input');
        cardNumErrorMsg.style.display = 'block';
    }    

    return !validationMsg;
}

function populateReviewFields(customShipType) {
    var countryElement = document.getElementById('flightshop-customer-country');
    var shipTypeElement = document.getElementById('flightshop-shipping-type');
    var addr1 = document.getElementById('flightshop-customer-addr-1').value;
    var addr2 = document.getElementById('flightshop-customer-addr-2').value || '(none)';
    var city = document.getElementById('flightshop-customer-city').value;
    var stateId = countryElement.value === 'US' ? 'flightshop-customer-state' : 'flightshop-customer-state-custom';
    var state = document.getElementById(stateId).value || '(none)';
    var zip = document.getElementById('flightshop-customer-zipcode').value;
    var country = countryElement.options[countryElement.selectedIndex].innerHTML;
    var shipTypeValue = shipTypeElement.value;
    var shipTypeText = shipTypeValue === customShipType.toString() ? document.getElementById('flightshop-customer-custom-shipping-type').value : shipTypeElement.options[shipTypeElement.selectedIndex].innerHTML;

    document.getElementById('flightshop-order-review-fname').innerHTML = document.getElementById('flightshop-customer-first-name').value;
    document.getElementById('flightshop-order-review-lname').innerHTML = document.getElementById('flightshop-customer-last-name').value;
    document.getElementById('flightshop-order-review-phone').innerHTML = document.getElementById('flightshop-customer-phone').value;
    document.getElementById('flightshop-order-review-email').innerHTML = document.getElementById('flightshop-customer-email').value;
    document.getElementById('flightshop-order-review-po-number').innerHTML = document.getElementById('flightshop-po-number').value || '(none)';
    document.getElementById('flightshop-order-review-company').innerHTML = document.getElementById('flightshop-customer-company-name').value || '(none)';
    document.getElementById('flightshop-order-review-attnto').innerHTML = document.getElementById('flightshop-customer-attention-to').value || '(none)'; 
    document.getElementById('flightshop-order-review-addr1').innerHTML = addr1;
    document.getElementById('flightshop-order-review-addr2').innerHTML = addr2;
    document.getElementById('flightshop-order-review-city').innerHTML = city;
    document.getElementById('flightshop-order-review-state').innerHTML = state;
    document.getElementById('flightshop-order-review-zip').innerHTML = zip;
    document.getElementById('flightshop-order-review-country').innerHTML = country;
    document.getElementById('flightshop-order-review-shiptype').innerHTML = shipTypeText;

    if (useShippingAddrForBilling()) {
        document.getElementById('flightshop-order-review-addr1-billing').innerHTML = addr1;
        document.getElementById('flightshop-order-review-addr2-billing').innerHTML = addr2;
        document.getElementById('flightshop-order-review-city-billing').innerHTML = city;
        document.getElementById('flightshop-order-review-state-billing').innerHTML = state;
        document.getElementById('flightshop-order-review-zip-billing').innerHTML = zip;
        document.getElementById('flightshop-order-review-country-billing').innerHTML = country;
    } else {
        document.getElementById('flightshop-order-review-addr1-billing').innerHTML = document.getElementById('flightshop-customer-addr-1-billing').value;
        document.getElementById('flightshop-order-review-addr2-billing').innerHTML = document.getElementById('flightshop-customer-addr-2-billing').value || '(none)';
        document.getElementById('flightshop-order-review-city-billing').innerHTML = document.getElementById('flightshop-customer-city-billing').value;
        var billingCountryElement = document.getElementById('flightshop-customer-country-billing');
        var billingStateId = billingCountryElement.value === 'US' ? 'flightshop-customer-state-billing' : 'flightshop-customer-state-custom-billing';
        document.getElementById('flightshop-order-review-state-billing').innerHTML = document.getElementById(billingStateId).value || '(none)';
        document.getElementById('flightshop-order-review-zip-billing').innerHTML = document.getElementById('flightshop-customer-zipcode-billing').value;
        document.getElementById('flightshop-order-review-country-billing').innerHTML = billingCountryElement.options[billingCountryElement.selectedIndex].innerHTML;
    }
}

function goBack(fromName, toName) {
    var fromSection = document.getElementById('flightshop-checkout-section-' + fromName);
    var toSection = document.getElementById('flightshop-checkout-section-' + toName);
    setTimeout(function () {
        if (toSection.style.display === 'none') {
            toSection.style.display = 'block';
        }
    }, 200);
    transitionCheckoutStep(fromName, toName, fromSection, toSection);

    var currentStepFields = fromSection.getElementsByClassName('mandatory-checkout-field');
    var previousStepFields = toSection.getElementsByClassName('mandatory-checkout-field');
    changeCurrentRequiredFields(currentStepFields, previousStepFields);
}

function validateFormComplete(requiredFields) {
    var isFormComplete = true;
    for (var i = 0; i < requiredFields.length && isFormComplete; i++) {
        isFormComplete = (requiredFields[i].value || requiredFields[i].checked) && !requiredFields[i].classList.contains('invalid-input');
        if (isFormComplete) {
            requiredFields[i].parentElement.classList.remove('has-error');
        } else {
            requiredFields[i].parentElement.classList.add('has-error');
        }
    }
    return isFormComplete;
}

function goNext(fromName, toName) {
    var fromSection = document.getElementById('flightshop-checkout-section-' + fromName);
    var toSection = document.getElementById('flightshop-checkout-section-' + toName);

    var currentStepFields = fromSection.getElementsByClassName('mandatory-checkout-field');
    var isFormComplete = validateFormComplete(currentStepFields);

    if (toSection.style.display === 'none') {
        toSection.style.display = 'block';
    }
    if (isFormComplete) {
        transitionCheckoutStep(fromName, toName, fromSection, toSection);
        var nextStepFields = toSection.getElementsByClassName('mandatory-checkout-field');
        changeCurrentRequiredFields(currentStepFields, nextStepFields);
    }
}

function changeCurrentRequiredFields(fieldsNotRequired, fieldsRequired) {
    for (var i = 0; i < fieldsRequired.length; i++) {
        fieldsRequired[i].setAttribute('required', true);
    }
    for (var j = 0; j < fieldsNotRequired.length; j++) {
        fieldsNotRequired[j].removeAttribute('required');
    }
}

function transitionCheckoutStep(fromName, toName, fromSection, toSection) {
    fromSection.classList.remove('section-fade-in');
    fromSection.classList.add('section-fade-out');
    
    var fromStep = document.getElementById('checkout-step-' + fromName);
    var toStep = document.getElementById('checkout-step-' + toName);

    setTimeout(function () {
        fromSection.style.display = 'none';
        fromStep.classList.remove('active', 'step-error');
        toStep.classList.add('active');

        if (fromName === 'shipping') {
            document.getElementById('checkout-truck-active').style.display = 'none';
            document.getElementById('checkout-truck-inactive').style.display = 'block';
        } else if (toName === 'shipping') {
            document.getElementById('checkout-truck-inactive').style.display = 'none';
            document.getElementById('checkout-truck-active').style.display = 'block';
        }
        toSection.classList.remove('section-fade-out');
        toSection.classList.add('section-fade-in');
        window.scrollTo(0, 0);
    }, 200);
}

function setPaymentAuthAmountText(cart) {
    var authAmount = 0;
    for (var i = 0; i < cart.length; i++) {
        authAmount += (cart[i].Price || 0) * (cart[i].Quantity || 0);
    }
    document.getElementById('flightshop-cc-amount-text').innerHTML = 'Your card will be pre-authorized for <strong>$' + authAmount.toFixed(2) +
        '</strong> when you press "Submit Order" on the following page, and we will get in touch with you to confirm purchase details before processing payment.';
}

function setOrderReviewTable(cart) {
    var table = document.getElementById('flightshop-order-review-table-body');
    var total = 0;
    for (var i = 0; i < cart.length; i++) {
        var item = cart[i];
        var itemPrice = item.Price || 0;
        var partNumber = document.createElement('td');
        partNumber.innerHTML = item.PartNumber;
        var description = document.createElement('td');
        description.innerHTML = item.Description || '';
        var price = document.createElement('td');
        price.style.textAlign = 'right';
        price.innerHTML = itemPrice ? '$' + itemPrice.toFixed(2) : '(quote)';
        var quantity = document.createElement('td');
        quantity.style.textAlign = 'right';
        quantity.innerHTML = item.Quantity;

        var subTotalAmount = (itemPrice * item.Quantity); 
        total += subTotalAmount;
        var subTotal = document.createElement('td');
        subTotal.style.textAlign = 'right';
        subTotal.innerHTML = itemPrice ? '$' + subTotalAmount.toFixed(2) : '(quote)';

        var lineItem = document.createElement('tr');
        lineItem.appendChild(partNumber);
        lineItem.appendChild(description);
        lineItem.appendChild(price);
        lineItem.appendChild(quantity);
        lineItem.appendChild(subTotal);
        table.appendChild(lineItem);
    }
    document.getElementById('flightshop-review-total-th').innerHTML = total > 0 ? '$' + total.toFixed(2) : '(quote)'
}

function clearSessionStorage() {
    window.sessionStorage.removeItem('flightshop-customer-first-name');
    window.sessionStorage.removeItem('flightshop-customer-last-name');
    window.sessionStorage.removeItem('flightshop-customer-phone');
    window.sessionStorage.removeItem('flightshop-customer-email');
    window.sessionStorage.removeItem('flightshop-shipping-type');
    window.sessionStorage.removeItem('flightshop-customer-custom-shipping-type');
    window.sessionStorage.removeItem('flightshop-po-number');
    window.sessionStorage.removeItem('flightshop-customer-company-name');
    window.sessionStorage.removeItem('flightshop-customer-attention-to');
    window.sessionStorage.removeItem('flightshop-customer-addr-1');
    window.sessionStorage.removeItem('flightshop-customer-addr-2');
    window.sessionStorage.removeItem('flightshop-customer-city');
    window.sessionStorage.removeItem('flightshop-customer-state');
    window.sessionStorage.removeItem('flightshop-customer-zipcode');
    window.sessionStorage.removeItem('flightshop-customer-country');
    window.sessionStorage.removeItem('flightshop-customer-addr-1-billing');
    window.sessionStorage.removeItem('flightshop-customer-addr-2-billing');
    window.sessionStorage.removeItem('flightshop-customer-city-billing');
    window.sessionStorage.removeItem('flightshop-customer-state-billing');
    window.sessionStorage.removeItem('flightshop-customer-zipcode-billing');
    window.sessionStorage.removeItem('flightshop-customer-country-billing');
    window.sessionStorage.removeItem('flightshop-cust-notes');
}