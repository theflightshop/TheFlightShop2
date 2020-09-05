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
        clientOrder.BillingState = clientOrder.CountryCode === 'US' ? document.getElementById('flightshop-customer-state-billing').value : document.getElementById('flightshop-customer-state-custom-billing').value;
        clientOrder.BillingZip = document.getElementById('flightshop-customer-zipcode-billing').value;
    }
}

function limitCreditCardExpLength(input) {
    if (input.value && input.value.length > 4) {
        input.value = input.value.slice(0, 4);
    }
}

function saveInputValueToStorage(id, storageKey) {
    var value = document.getElementById(id).value;
    window.sessionStorage.setItem(storageKey, value);
}

function saveInputValuesToStorage() {
    saveInputValueToStorage('flightshop-customer-first-name', 'first-name');
    saveInputValueToStorage('flightshop-customer-last-name', 'last-name');
    saveInputValueToStorage('flightshop-customer-phone', 'customer-phone');
    saveInputValueToStorage('flightshop-customer-email', 'customer-email');
    saveInputValueToStorage('flightshop-shipping-type', 'shipping-type');
    saveInputValueToStorage('flightshop-po-number', 'po-number');
    saveInputValueToStorage('flightshop-customer-attention-to', 'attention-to');
    saveInputValueToStorage('flightshop-customer-addr-1', 'addr-1');
    saveInputValueToStorage('flightshop-customer-addr-2', 'addr-2');
    saveInputValueToStorage('flightshop-customer-city', 'customer-city');
    saveInputValueToStorage('flightshop-customer-state', 'customer-state');
    saveInputValueToStorage('flightshop-customer-zipcode', 'customer-zipcode');
    saveInputValueToStorage('flightshop-customer-country', 'customer-country');
    saveInputValueToStorage('flightshop-customer-addr-1-billing', 'addr-1-billing');
    saveInputValueToStorage('flightshop-customer-addr-2-billing', 'addr-2-billing');
    saveInputValueToStorage('flightshop-customer-city-billing', 'city-billing');
    saveInputValueToStorage('flightshop-customer-state-billing', 'state-billing');
    saveInputValueToStorage('flightshop-customer-zipcode-billing', 'zipcode-billing');
    saveInputValueToStorage('flightshop-customer-country-billing', 'country-billing');
}

function submitCustomerInfo(customerInfoUrl, errorRedirectUrl, customShipType, formUrlReturned) {
    var shippingType = parseInt(document.getElementById('flightshop-shipping-type').value);
    var clientOrder = {
        FirstName: document.getElementById('flightshop-customer-first-name').value,
        LastName: document.getElementById('flightshop-customer-last-name').value,
        Email: document.getElementById('flightshop-customer-email').value,
        Phone: document.getElementById('flightshop-customer-phone').value,

        ShippingType: shippingType,
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
            Quantity: parseInt(item.Quantity) || 0
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
        formUrlReturned(response);
    }).fail(function () {
        location.href = errorRedirectUrl;
    });
}

function submitOrder(customerInfoUrl, errorRedirectUrl, customShipType) {
    var agreed = document.getElementById('flightshop-order-agree-checkbox');
    if (agreed.checked) {
        document.getElementById('flightshop-checkout-section-review').style.display = 'none';
        document.getElementById('flightshop-checkout-steps-header').style.display = 'none';
        document.getElementById('flightshop-checkout-title').style.display = 'none';
        document.getElementById('flightshop-checkout-section-processing').style.display = 'block';
        submitCustomerInfo(customerInfoUrl, errorRedirectUrl, customShipType, function (formUrl) {
            var cardForm = document.getElementById('flightshop-checkout-form');
            cardForm.action = formUrl;
            cardForm.submit();
        });
    } else {
        agreed.parentElement.classList.add('has-error');
    }
}

function paymentInfoSubmitted() {
    var cardExp = document.getElementById('billing-cc-exp');
    var validationMsg = null;
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
        document.getElementById('billing-cc-exp-validation-msg').innerHTML = validationMsg;
        cardExp.classList.add('invalid-input');
    } else {
        cardExp.classList.remove('invalid-input');
    }

    goNext('payment', 'review');
}

function goBack(fromName, toName) {
    var fromSection = document.getElementById('flightshop-checkout-section-' + fromName);
    var toSection = document.getElementById('flightshop-checkout-section-' + toName);
    transitionCheckoutStep(fromName, toName, fromSection, toSection);

    var currentStepFields = fromSection.getElementsByClassName('mandatory-checkout-field');
    var previousStepFields = toSection.getElementsByClassName('mandatory-checkout-field');
    changeCurrentRequiredFields(currentStepFields, previousStepFields);
}

function goNext(fromName, toName) {
    var fromSection = document.getElementById('flightshop-checkout-section-' + fromName);
    var toSection = document.getElementById('flightshop-checkout-section-' + toName);

    var currentStepFields = fromSection.getElementsByClassName('mandatory-checkout-field');
    var isFormComplete = true;
    for (var i = 0; i < currentStepFields.length && isFormComplete; i++) {
        isFormComplete = currentStepFields[i].value && !currentStepFields[i].classList.contains('invalid-input');
        if (isFormComplete) {
            currentStepFields[i].parentElement.classList.remove('has-error');
        } else {
            currentStepFields[i].parentElement.classList.add('has-error');
        }
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

        if (toSection.style.display === 'none') {
            toSection.style.display = 'block';
        }

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

function setPaymentAuthAmountText() {
    var cart = JSON.parse(window.sessionStorage.getItem('cartItems'));
    console.log('cart ', cart)
    var authAmount = 0;
    for (var i = 0; i < cart.length; i++) {
        authAmount += (cart[i].Price || 0) * (cart[i].Quantity || 0);
    }
    document.getElementById('flightshop-cc-amount-text').innerHTML = 'Your card will be authorized for <strong>$' + authAmount.toFixed(2) +
        '</strong> when you press "Submit Order" on the following page, and we will get in touch with you to confirm purchase details before processing payment.';
}

$(document).ready(function () {
    var checkoutForm = document.getElementById('flightshop-checkout-form');
    if (checkoutForm) {
        document.getElementById('flightshop-customer-country').value = 'US';
        document.getElementById('flightshop-customer-country-billing').value = 'US';
        $('#flightshop-customer-country').select2();
        $('#flightshop-customer-country-billing').select2();

        setPaymentAuthAmountText();

        checkoutForm.addEventListener('submit', function (evt) {
            evt.preventDefault();
        });
    }
});