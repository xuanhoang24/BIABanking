// Transfer page functionality
const transferType = document.getElementById("transferType");
const internalSection = document.getElementById("internalTransferSection");
const externalSection = document.getElementById("externalTransferSection");
const fromSelect = document.getElementById("fromAccountSelect");
const toSelect = document.getElementById("toAccountSelect");

function updateTransferType() {
    if (transferType.value === "Internal") {
        internalSection.style.display = "block";
        externalSection.style.display = "none";
    } else {
        internalSection.style.display = "none";
        externalSection.style.display = "block";
    }
}

function updateInternalOptions() {
    const fromValue = fromSelect.value;
    Array.from(toSelect.options).forEach(opt => {
        if (!opt.value) return;
        opt.hidden = opt.value === fromValue;
    });
}

transferType.addEventListener("change", updateTransferType);
fromSelect.addEventListener("change", updateInternalOptions);

updateTransferType();
