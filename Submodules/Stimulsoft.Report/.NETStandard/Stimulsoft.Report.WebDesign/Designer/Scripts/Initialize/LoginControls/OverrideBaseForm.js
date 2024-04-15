
StiMobileDesigner.prototype.OverrideBaseForm = function () {

	this.OldBaseForm = this.BaseForm;
	this.BaseForm = function (name, caption, level, helpUrl, centerForm, isModal) {
		var form = this.OldBaseForm(name, caption, level, helpUrl);
		form.buttonSave = form.buttonOk;
		form.buttonsTable = form.buttonsPanel.firstChild;

		form.addControlRow2 = function (text, controlName, control, margin) {
			if (form.controlsTable == null) {
				form.controlsTable = form.jsObject.CreateHTMLTable();
				form.controlsTable.style.width = "100%";
				form.container.appendChild(form.controlsTable);
			}

			form.controlsTable.addRow();
			var textCell = form.controlsTable.addCellInLastRow();
			textCell.innerHTML = text;
			textCell.className = "stDesignerFormTextBeforeControl";
			if (!form.controls) form.controls = {};
			form.controls[controlName] = control;
			control.style.margin = margin;
			var controlCell = form.controlsTable.addCellInLastRow(control);
			controlCell.style.width = "1px";

			return controlCell;
		}

		if (centerForm) {
			$(window).resize(function () {
				form.jsObject.SetObjectToCenter(form);
			});
		}

		return form;
	}
}