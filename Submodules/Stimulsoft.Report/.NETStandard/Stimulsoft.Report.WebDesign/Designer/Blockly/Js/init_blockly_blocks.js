
function initBlocklyBlocks() {
    //Visuals
    Blockly.Blocks['sti_color_hex'] = {
        init: function () {
            this.appendValueInput("TEXT")
                .setCheck(null)
                .appendField("Color Hex");
            this.setOutput(true, null);
            this.setColour("#f3aa60");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };


    Blockly.Blocks['sti_color_argb'] = {
        init: function () {
            this.appendDummyInput()
                .appendField("Color");
            this.appendValueInput("ALPHA")
                .setCheck("Number")
                .appendField("A");
            this.appendValueInput("RED")
                .setCheck("Number")
                .appendField("R");
            this.appendValueInput("GREEN")
                .setCheck("Number")
                .appendField("G");
            this.appendValueInput("BLUE")
                .setCheck("Number")
                .appendField("B");
            this.setInputsInline(true);
            this.setOutput(true, null);
            this.setColour("#f3aa60");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_new_pen_style'] = {
        init: function () {
            this.appendDummyInput()
                .appendField(new Blockly.FieldDropdown([[{ "src": "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAB8AAAAMCAIAAAAVjvFbAAAABGdBTUEAAK/INwWK6QAAABl0RVh0U29mdHdhcmUAQWRvYmUgSW1hZ2VSZWFkeXHJZTwAAAAtSURBVHjaYmKgJWAB4v///9PCaEZGRiaaun0om84CCaAh6/bRNDPc0gxAgAEA2PcGJxNMwcUAAAAASUVORK5CYII=", "width": 31, "height": 12, "alt": "SOLID" }, "SOLID"], [{ "src": "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAB8AAAAMCAIAAAAVjvFbAAAABGdBTUEAAK/INwWK6QAAABl0RVh0U29mdHdhcmUAQWRvYmUgSW1hZ2VSZWFkeXHJZTwAAAAvSURBVHjaYmCgJWAE4v///9PEaEZGJpq6fSibjh38BwPKxZlG08yISTPUAgABBgBcvxf/vGsUdQAAAABJRU5ErkJggg==", "width": 31, "height": 12, "alt": "DASH" }, "DASH"], [{ "src": "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAB8AAAAMCAIAAAAVjvFbAAAABGdBTUEAAK/INwWK6QAAABl0RVh0U29mdHdhcmUAQWRvYmUgSW1hZ2VSZWFkeXHJZTwAAAAxSURBVHjaYmCgJWAE4v///9PEaEZGJpq6fSibjh38BwNMNqlqmEbTzIhJM9QCAAEGAJhIHfmAnP3IAAAAAElFTkSuQmCC", "width": 31, "height": 12, "alt": "DASHDOT" }, "DASHDOT"], [{ "src": "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAB8AAAAMCAIAAAAVjvFbAAAABGdBTUEAAK/INwWK6QAAABl0RVh0U29mdHdhcmUAQWRvYmUgSW1hZ2VSZWFkeXHJZTwAAABASURBVHjaYmKgJWAB4v///9PCaEZGRiaaun0om84CCSA0UUhMQMRxsYlRzzRgaQZZHBcbj/rRNENymqEWAAgwAG/6JBjYKqFlAAAAAElFTkSuQmCC", "width": 31, "height": 12, "alt": "DASHDOTDOT" }, "DASHDOTDOT"], [{ "src": "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAB8AAAAMCAIAAAAVjvFbAAAABGdBTUEAAK/INwWK6QAAABl0RVh0U29mdHdhcmUAQWRvYmUgSW1hZ2VSZWFkeXHJZTwAAAA5SURBVHjaYmKgJWAB4v///9PCaEZGRiaaun2om84IBvDAohab5m5HTzNUZI+mGQLhDo9oqgOAAAMATKobOjmU4hMAAAAASUVORK5CYII=", "width": 31, "height": 12, "alt": "DOT" }, "DOT"], [{ "src": "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAB8AAAAMCAIAAAAVjvFbAAAABGdBTUEAAK/INwWK6QAAABl0RVh0U29mdHdhcmUAQWRvYmUgSW1hZ2VSZWFkeXHJZTwAAAAqSURBVHjaYmCgJWAE4v///9PEaEZGJpq6fSibzjCEY5VhNEUOwzQDEGAAFugJDkQsdeUAAAAASUVORK5CYII=", "width": 31, "height": 12, "alt": "DOUBLE" }, "DOUBLE"], [{ "src": "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAB8AAAAMCAIAAAAVjvFbAAAABGdBTUEAAK/INwWK6QAAABl0RVh0U29mdHdhcmUAQWRvYmUgSW1hZ2VSZWFkeXHJZTwAAAArSURBVHjaYmKgJWAB4v///9PCaEZGRiaaun3U9FHTR5LpLJBMRSPTAQIMAFY6AyA7ubzVAAAAAElFTkSuQmCC", "width": 31, "height": 12, "alt": "NONE" }, "NONE"]]), "VALUE");
            this.setOutput(true, null);
            this.setColour("#f3aa60");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_new_border'] = {
        init: function () {
            this.appendDummyInput()
                .appendField("Border");
            this.appendDummyInput()
                .setAlign(Blockly.ALIGN_CENTRE)
                .appendField(new Blockly.FieldCheckbox("TRUE"), "TOP");
            this.appendDummyInput()
                .appendField(new Blockly.FieldCheckbox("TRUE"), "LEFT")
                .appendField(new Blockly.FieldImage("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQEAQAAADlauupAAAABGdBTUEAALGPC/xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAAAmJLR0QAAKqNIzIAAAAJcEhZcwAAAGAAAABgAPBrQs8AAAAHdElNRQflBRoGOjUKjn7yAAAATElEQVQ4y+2TMREAIAwDUyxwCEF9ZeCjHBqekRlYGPg9nywxiJBK0RG9G4A0hmhtK2u1SjkLANx3u8EdIJ1NX3zBF7whuD6T3d05YgJGMiyFOuTm6gAAACV0RVh0ZGF0ZTpjcmVhdGUAMjAyMS0wNS0yNlQwNjo1ODo1MyswMDowMBn+UTkAAAAldEVYdGRhdGU6bW9kaWZ5ADIwMjEtMDUtMjZUMDY6NTg6NTMrMDA6MDBoo+mFAAAAAElFTkSuQmCC", 16, 16, { alt: "*", flipRtl: "FALSE" }))
                .appendField(new Blockly.FieldCheckbox("TRUE"), "RIGHT");
            this.appendDummyInput()
                .setAlign(Blockly.ALIGN_CENTRE)
                .appendField(new Blockly.FieldCheckbox("TRUE"), "BOTTOM");
            this.appendValueInput("COLOR")
                .setCheck(null)
                .setAlign(Blockly.ALIGN_RIGHT)
                .appendField("Color");
            this.appendValueInput("SIZE")
                .setCheck(null)
                .setAlign(Blockly.ALIGN_RIGHT)
                .appendField("Size");
            this.appendValueInput("STYLE")
                .setCheck(null)
                .setAlign(Blockly.ALIGN_RIGHT)
                .appendField("Style");
            this.setInputsInline(false);
            this.setOutput(true, null);
            this.setColour("#f3aa60");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_new_brush'] = {
        validate: function (newValue) {
            this.getSourceBlock().updateConnections(newValue);
            return newValue;
        },
        init: function () {
            this.appendDummyInput()
                .appendField("Brush")
                .appendField(new Blockly.FieldDropdown([["empty", "EMPTYBRUSH"]], this.validate), "VALUE");
            this.setOutput(true, null);
            this.setColour("#f3aa60");
            this.setTooltip("");
            this.setHelpUrl("");
        },
        updateConnections: function (newValue) {
            this.removeInput('COLOR', true);
            this.removeInput('ANGLE', true);
            this.removeInput('STARTCOLOR', true);
            this.removeInput('ENDCOLOR', true);

            if (newValue == 'Solid') {
                this.appendValueInput("COLOR")
                    .setCheck(null)
                    .setAlign(Blockly.ALIGN_RIGHT)
                    .appendField("Color");

            } else if (newValue == 'Gradient') {
                this.appendValueInput("ANGLE")
                    .setCheck("Number")
                    .setAlign(Blockly.ALIGN_RIGHT)
                    .appendField("Angle");
                this.appendValueInput("STARTCOLOR")
                    .setCheck(null)
                    .setAlign(Blockly.ALIGN_RIGHT)
                    .appendField("Start Color");
                this.appendValueInput("ENDCOLOR")
                    .setCheck(null)
                    .setAlign(Blockly.ALIGN_RIGHT)
                    .appendField("End Color");
            }
        }
    };

    Blockly.Blocks['sti_new_gradient_brush'] = {
        init: function () {
            this.appendDummyInput()
                .appendField("Gradient Brush");
            this.appendValueInput("ANGLE")
                .setCheck("Number")
                .setAlign(Blockly.ALIGN_RIGHT)
                .appendField("Angle");
            this.appendValueInput("STARTCOLOR")
                .setCheck(null)
                .setAlign(Blockly.ALIGN_RIGHT)
                .appendField("Start Color");
            this.appendValueInput("ENDCOLOR")
                .setCheck(null)
                .setAlign(Blockly.ALIGN_RIGHT)
                .appendField("End Color");
            this.setOutput(true, null);
            this.setColour("#f3aa60");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_new_solid_brush'] = {
        init: function () {
            this.appendDummyInput()
                .appendField("Solid Brush");
            this.appendValueInput("COLOR")
                .setCheck(null)
                .setAlign(Blockly.ALIGN_RIGHT)
                .appendField("Color");
            this.setOutput(true, null);
            this.setColour("#f3aa60");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_new_font'] = {
        init: function () {
            this.appendDummyInput()
                .appendField("Font");
            this.appendDummyInput()
                .setAlign(Blockly.ALIGN_RIGHT)
                .appendField(new Blockly.FieldDropdown([["empty", "EMPTY"]]), "NAME");
            this.appendDummyInput()
                .setAlign(Blockly.ALIGN_RIGHT)
                .appendField("Bold")
                .appendField(new Blockly.FieldCheckbox("TRUE"), "BOLD")
                .appendField("Italic")
                .appendField(new Blockly.FieldCheckbox("TRUE"), "ITALIC");
            this.appendDummyInput()
                .setAlign(Blockly.ALIGN_RIGHT)
                .appendField("Underline")
                .appendField(new Blockly.FieldCheckbox("TRUE"), "UNDERLINE")
                .appendField("Strikeout")
                .appendField(new Blockly.FieldCheckbox("TRUE"), "STRIKEOUT");
            this.appendValueInput("SIZE")
                .setCheck(null)
                .setAlign(Blockly.ALIGN_RIGHT)
                .appendField("Size");
            this.setOutput(true, null);
            this.setColour("#f3aa60");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_new_margin'] = {
        init: function () {
            this.appendValueInput("LEFT")
                .setCheck(null)
                .appendField("Margin")
                .appendField("left");
            this.appendValueInput("TOP")
                .setCheck(null)
                .appendField("top");
            this.appendValueInput("RIGHT")
                .setCheck(null)
                .appendField("right");
            this.appendValueInput("BOTTOM")
                .setCheck(null)
                .appendField("bottom");
            this.setInputsInline(true);
            this.setOutput(true, null);
            this.setColour("#f3aa60");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_new_padding'] = {
        init: function () {
            this.appendValueInput("LEFT")
                .setCheck(null)
                .appendField("Padding")
                .appendField("left");
            this.appendValueInput("TOP")
                .setCheck(null)
                .appendField("top");
            this.appendValueInput("RIGHT")
                .setCheck(null)
                .appendField("right");
            this.appendValueInput("BOTTOM")
                .setCheck(null)
                .appendField("bottom");
            this.setInputsInline(true);
            this.setOutput(true, null);
            this.setColour("#f3aa60");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };


    Blockly.Blocks['sti_new_corner_radius'] = {
        init: function () {
            this.appendValueInput("TOPLEFT")
                .setCheck(null)
                .appendField("Corner Radius")
                .appendField("top left");
            this.appendValueInput("TOPRIGHT")
                .setCheck(null)
                .appendField("top right");
            this.appendValueInput("BOTTOMRIGHT")
                .setCheck(null)
                .appendField("bottom right");
            this.appendValueInput("BOTTOMLEFT")
                .setCheck(null)
                .appendField("bottom left");
            this.setInputsInline(true);
            this.setOutput(true, null);
            this.setColour("#f3aa60");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    //Objects
    Blockly.Blocks['sti_this_component'] = {
        init: function () {
            this.appendDummyInput()
                .appendField("This Component");
            this.setOutput(true, null);
            this.setColour("#00b6ad");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_all_components'] = {
        init: function () {
            this.appendDummyInput()
                .appendField("All Components");
            this.setOutput(true, null);
            this.setColour("#00b6ad");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_all_components_from'] = {
        init: function () {
            this.appendDummyInput()
                .appendField("All Components from")
                .appendField(new Blockly.FieldDropdown([["ComponentGetComponents", "EMPTYCOMPONENTGETCOMPONENTS"]]), "NAME");
            this.setOutput(true, null);
            this.setColour("#00b6ad");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_this_report'] = {
        init: function () {
            this.appendDummyInput()
                .appendField("This Report");
            this.setOutput(true, null);
            this.setColour("#00b6ad");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_get_current_value'] = {
        init: function () {
            this.appendDummyInput()
                .appendField("Get Current Value");
            this.setOutput(true, null);
            this.setColour("#00b6ad");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_set_current_value'] = {
        init: function () {
            this.appendValueInput("VALUE")
                .setCheck(null)
                .appendField("Set Current Value");
            this.setPreviousStatement(true, null);
            this.setNextStatement(true, null);
            this.setColour("#00b6ad");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_get_component'] = {
        init: function () {
            this.appendDummyInput()
                .appendField(new Blockly.FieldDropdown([["Component", "EMPTYCOMPONENT"]]), "NAME");
            this.setOutput(true, null);
            this.setColour("#00b6ad");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_get_component_by_name'] = {
        init: function () {
            this.appendValueInput("NAME")
                .setCheck("String")
                .appendField("Get Component by Name");
            this.setInputsInline(false);
            this.setOutput(true, null);
            this.setColour("#00b6ad");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_set_property_to_value'] = {
        init: function () {
            this.appendValueInput("COMPONENT")
                .setCheck(null)
                .appendField("Set");
            this.appendValueInput("VALUE")
                .setCheck(null)
                .appendField("`s")
                .appendField(new Blockly.FieldDropdown([["Enabled", "ENABLED"], ["Background", "BACKGROUND"], ["Width", "WIDTH"], ["Height", "HEIGHT"]]), "PROPERTY")
                .appendField("to");
            this.setInputsInline(true);
            this.setPreviousStatement(true, null);
            this.setNextStatement(true, null);
            this.setColour("#00b6ad");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_get_style_by_name'] = {
        init: function () {
            this.appendValueInput("VALUE")
                .setCheck("String")
                .appendField("Get Style by Name");
            this.setOutput(true, null);
            this.setColour("#00b6ad");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_get_property_of_object'] = {
        init: function () {
            this.appendValueInput("PROPERTY")
                .setCheck("String")
                .appendField("Get Property");
            this.appendValueInput("OBJECT")
                .setCheck(null)
                .appendField("of Object");
            this.setOutput(true, null);
            this.setColour("#00b6ad");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_set_property_of_object_to'] = {
        init: function () {
            this.appendValueInput("PROPERTY")
                .setCheck("String")
                .setAlign(Blockly.ALIGN_RIGHT)
                .appendField("Set Property");
            this.appendValueInput("OBJECT")
                .setCheck(null)
                .setAlign(Blockly.ALIGN_RIGHT)
                .appendField("of Object");
            this.appendValueInput("VALUE")
                .setCheck(null)
                .setAlign(Blockly.ALIGN_RIGHT)
                .appendField("to");
            this.setPreviousStatement(true, null);
            this.setNextStatement(true, null);
            this.setColour("#00b6ad");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    //Data
    Blockly.Blocks['sti_get_data_source_by_name'] = {
        init: function () {
            this.appendValueInput("NAME")
                .setCheck("String")
                .appendField("Get Data Source by Name");
            this.setOutput(true, null);
            this.setColour("#415c71");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_get_data_source'] = {
        init: function () {
            this.appendDummyInput()
                .appendField(new Blockly.FieldDropdown([["DataSource", "EMPTYDATASOURCE"]]), "NAME");
            this.setOutput(true, null);
            this.setColour("#415c71");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_data_source_property'] = {
        init: function () {
            this.appendDummyInput()
                .appendField("Get Property")
                .appendField(new Blockly.FieldDropdown([["empty", "EMPTYDATASOURCEPROPERTY"]]), "NAME");
            this.appendValueInput("OBJECT")
                .setCheck(null)
                .setAlign(Blockly.ALIGN_RIGHT)
                .appendField("of Data Source");
            this.setOutput(true, null);
            this.setColour("#415c71");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_data_source_method'] = {
        init: function () {
            this.appendValueInput("OBJECT")
                .setCheck(null)
                .setAlign(Blockly.ALIGN_RIGHT)
                .appendField("Data Source");
            this.appendDummyInput()
                .appendField(new Blockly.FieldDropdown([["empty", "EMPTYDATASOURCEMETHODS"]]), "NAME");
            this.setPreviousStatement(true, null);
            this.setNextStatement(true, null);
            this.setColour("#415c71");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_set_data_source_sql_command'] = {
        init: function () {
            this.appendValueInput("DATA")
                .setCheck(null)
                .appendField("Set Sql Command");
            this.appendValueInput("VALUE")
                .setCheck("String")
                .appendField("'s to");
            this.setInputsInline(true);
            this.setPreviousStatement(true, null);
            this.setNextStatement(true, null);
            this.setColour("#415c71");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_data_source_get_data'] = {
        init: function () {
            this.appendValueInput("DATA")
                .setCheck(null)
                .appendField("Get Data");
            this.appendValueInput("COLUMN")
                .setCheck("String")
                .appendField("column name");
            this.appendValueInput("ROW")
                .setCheck("Number")
                .appendField("index of row");
            this.setOutput(true, null);
            this.setColour("#415c71");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    //Variable
    Blockly.Blocks['sti_get_variable_by_name'] = {
        init: function () {
            this.appendValueInput("VALUE")
                .setCheck("String")
                .appendField("Get Value");
            this.setOutput(true, null);
            this.setColour("#814968");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_get_variable'] = {
        init: function () {
            this.appendDummyInput()
                .appendField("Get Value")
                .appendField(new Blockly.FieldDropdown([["Variable", "EMPTYVARIABLE"]]), "VALUE");
            this.setOutput(true, null);
            this.setColour("#814968");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_set_variable'] = {
        init: function () {
            this.appendValueInput("NAME")
                .setCheck(null)
                .setAlign(Blockly.ALIGN_RIGHT)
                .appendField("Set Variable");
            this.appendValueInput("VALUE")
                .setCheck(null)
                .setAlign(Blockly.ALIGN_RIGHT)
                .appendField("to");
            this.setPreviousStatement(true, null);
            this.setNextStatement(true, null);
            this.setColour("#814968");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_system_variable'] = {
        init: function () {
            this.appendDummyInput()
                .appendField("System Variable")
                .appendField(new Blockly.FieldDropdown([["empty", "EMPTYSYSTEMVARIABLE"]]), "NAME");
            this.setOutput(true, null);
            this.setColour("#814968");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    //System
    Blockly.Blocks['sti_is_first_pass'] = {
        init: function () {
            this.appendDummyInput()
                .appendField("Is First Pass");
            this.setOutput(true, null);
            this.setColour("#adadad");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_is_second_pass'] = {
        init: function () {
            this.appendDummyInput()
                .appendField("Is Second Pass");
            this.setOutput(true, null);
            this.setColour("#adadad");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    //Process
    Blockly.Blocks['sti_open_link'] = {
        init: function () {
            this.appendValueInput("VALUE")
                .setCheck(null)
                .appendField("Open Link");
            this.setInputsInline(true);
            this.setPreviousStatement(true, null);
            this.setNextStatement(true, null);
            this.setColour("#00b1e1");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_refresh_viewer'] = {
        init: function () {
            this.appendDummyInput()
                .appendField("Refresh Viewer");
            this.setPreviousStatement(true, null);
            this.setNextStatement(true, null);
            this.setColour("#00b1e1");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    Blockly.Blocks['sti_show_message'] = {
        init: function () {
            this.appendValueInput("TEXT")
                .setCheck(null)
                .appendField("Show Message");
            this.setPreviousStatement(true, null);
            this.setNextStatement(true, null);
            this.setColour("#00b1e1");
            this.setTooltip("");
            this.setHelpUrl("");
        }
    };

    //Functions
    /*functions*/
}