document.addEventListener('DOMContentLoaded', function () {

    function updateTotals() {
        let gross = 0;
        let totalDeduction = 0;
        let totalContribution = 0;

        // Calculate Gross from Earning Items
        document.querySelectorAll("#earningItemsBody tr").forEach(row => {
            const rate = parseFloat(row.querySelector("[name*='Rate']")?.value || 0);
            const days = parseFloat(row.querySelector("[name*='Days']")?.value || 0);
            const hours = parseFloat(row.querySelector("[name*='Hours']")?.value || 0);
            const minutes = parseFloat(row.querySelector("[name*='Minutes']")?.value || 0);

            let amount = (rate * days) + ((rate / 8) * hours) + ((rate / 480) * minutes);
            amount = Math.round(amount * 100) / 100;

            const amountField = row.querySelector("[name*='Amount']");
            if (amountField) amountField.value = amount.toFixed(2);

            gross += amount;
        });

        // Calculate Total Deductions
        document.querySelectorAll("#deductionItemsBody tr").forEach(row => {
            const amount = parseFloat(row.querySelector("[name*='Amount']")?.value || 0);
            totalDeduction += amount;
        });

        // Calculate Total Contributions
        document.querySelectorAll("#contributionItemsBody tr").forEach(row => {
            const amountInput = row.querySelector("[name*='Amount']");
            const type = row.querySelector("[name*='ContributionType']")?.value || "Absolute";

            let amount = parseFloat(amountInput?.value || 0);

            if (type === "Percentage") {
                amount = (gross * amount) / 100;
                amount = Math.round(amount * 100) / 100;
                if (amountInput) amountInput.value = amount.toFixed(2);
            }

            totalContribution += amount;
        });

        // Update Total Fields
        document.querySelector("[name='GrossPay']").value = gross.toFixed(2);
        document.querySelector("[name='TotalDeduction']").value = totalDeduction.toFixed(2);
        document.querySelector("[name='TotalContribution']").value = totalContribution.toFixed(2);

        const netPayInput = document.querySelector("[name='NetPay']");
        if (netPayInput) netPayInput.value = (gross - totalDeduction - totalContribution).toFixed(2);
    }

    function refreshIndexes(tableId, prefix) {
        document.querySelectorAll(`#${tableId} tr`).forEach((row, i) => {
            row.querySelectorAll("input, select").forEach(input => {
                const oldName = input.name;
                const parts = oldName.split('.');
                if (parts.length === 2) {
                    input.name = `${prefix}[${i}].${parts[1]}`;
                }
            });
        });
    }

    // Recalculate on any input
    document.body.addEventListener("input", function (e) {
        if (
            e.target.closest("#earningItemsBody") ||
            e.target.closest("#deductionItemsBody") ||
            e.target.closest("#contributionItemsBody")
        ) {
            updateTotals();
        }
    });

    // Add Earning Item
    document.getElementById('addEarningItemBtn')?.addEventListener('click', function () {
        const index = document.querySelectorAll("#earningItemsBody tr").length;
        const row = document.createElement("tr");
        row.innerHTML = `
            <td>
                <select name="EarningItems[${index}].EarningAccountId" class="form-select" required>
                    <option value="">-- Select --</option>
                    ${earningOptions}
                </select>
            </td>
            <td><input name="EarningItems[${index}].Rate" value="0" class="form-control" type="number" required /></td>
            <td><input name="EarningItems[${index}].Days" value="0" class="form-control" type="number" /></td>
            <td><input name="EarningItems[${index}].Hours" value="0" class="form-control" type="number" /></td>
            <td><input name="EarningItems[${index}].Minutes" value="0" class="form-control" type="number" /></td>
            <td><input name="EarningItems[${index}].Description" class="form-control" /></td>
            <td><input name="EarningItems[${index}].Amount" readonly class="form-control" /></td>
            <td><button type="button" class="btn btn-danger remove-earning-item">✖️</button></td>
        `;
        document.getElementById('earningItemsBody').appendChild(row);
    });

    document.getElementById('earningItemsBody')?.addEventListener('click', function (e) {
        if (e.target.classList.contains('remove-earning-item')) {
            e.target.closest('tr').remove();
            refreshIndexes("earningItemsBody", "EarningItems");
            updateTotals();
        }
    });

    // Add Deduction Item
    document.getElementById('addDeductionItemBtn')?.addEventListener('click', function () {
        const index = document.querySelectorAll("#deductionItemsBody tr").length;
        const row = document.createElement("tr");
        row.innerHTML = `
            <td>
                <select name="DeductionItems[${index}].DeductionAccountId" class="form-select" required>
                    <option value="">-- Select --</option>
                    ${deductionOptions}
                </select>
            </td>
            <td><input name="DeductionItems[${index}].Description" class="form-control" /></td>
            <td><input name="DeductionItems[${index}].Amount" value="0" class="form-control" type="number" required /></td>
            <td>
                <select name="DeductionItems[${index}].DeductionType" class="form-select">
                    <option value="Absolute">✖️ Absolute</option>
                    <option value="Percentage">%</option>
                </select>
            </td>
            <td><button type="button" class="btn btn-danger remove-deduction-item">✖️</button></td>
        `;
        document.getElementById('deductionItemsBody').appendChild(row);
    });

    document.getElementById('deductionItemsBody')?.addEventListener('click', function (e) {
        if (e.target.classList.contains('remove-deduction-item')) {
            e.target.closest('tr').remove();
            refreshIndexes("deductionItemsBody", "DeductionItems");
            updateTotals();
        }
    });

    // Add Contribution Item
    document.getElementById('addContributionItemBtn')?.addEventListener('click', function () {
        const index = document.querySelectorAll("#contributionItemsBody tr").length;
        const row = document.createElement("tr");
        row.innerHTML = `
            <td>
                <select name="ContributionItems[${index}].ContributionAccountId" class="form-select" required>
                    <option value="">-- Select --</option>
                    ${contributionOptions}
                </select>
            </td>
            <td><input name="ContributionItems[${index}].Description" class="form-control" /></td>
            <td><input name="ContributionItems[${index}].Amount" value="0" class="form-control" type="number" required /></td>
            <td>
                <select name="ContributionItems[${index}].ContributionType" class="form-select">
                    <option value="Absolute">✖️ Absolute</option>
                    <option value="Percentage">%</option>
                </select>
            </td>
            <td class="text-center">
                <input type="checkbox" name="ContributionItems[${index}].IsApplicableEveryMonth" class="form-check-input" />
            </td>
            <td><button type="button" class="btn btn-danger remove-contribution-item">✖️</button></td>
        `;
        document.getElementById('contributionItemsBody').appendChild(row);
    });

    document.getElementById('contributionItemsBody')?.addEventListener('click', function (e) {
        if (e.target.classList.contains('remove-contribution-item')) {
            e.target.closest('tr').remove();
            refreshIndexes("contributionItemsBody", "ContributionItems");
            updateTotals();
        }
    });

    updateTotals(); // Initial calculation
});
