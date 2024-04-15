animateSti = function (chartId) {
    var requestAnimationFrame = window.requestAnimationFrame || window.mozRequestAnimationFrame ||
        window.webkitRequestAnimationFrame || window.msRequestAnimationFrame;
    window.requestAnimationFrame = requestAnimationFrame;
    var animations = [];
    var chart_ = document.getElementById(chartId);

    var createTooltip = function () {
        var table = document.createElement("table");
        table.style.position = "absolute";
        table.style.opacity = "0";
        table.style.background = "white";
        table.style.padding = "5px";
        table.style.border = "1px solid #bebebe";
        table.style.fontFamily = "Arial";
        table.style.fontSize = "12px";
        table.style.color = "#111111";
        table.style.zIndex = "20000";
        table.style.pointerEvents = "none";
        table.style.display = "block";
        document.body.appendChild(table);
        document._stiTooltip = table;
        var tr = document.createElement("tr");
        table.appendChild(tr);
        var td = document.createElement("td");
        td.style.verticalAlign = "top";
        td.rowSpan = 2;
        table._round = document.createElement('div');
        table._round.style.width = "20px";
        table._round.style.height = "20px";
        table._round.style.borderRadius = "20px";
        td.appendChild(table._round);
        tr.appendChild(td);
        table._text1 = document.createElement("td");
        table._text1.style.paddingTop = "3px";
        tr.appendChild(table._text1);
        tr = document.createElement("tr");
        table.appendChild(tr);
        table._text2 = document.createElement("td");
        tr.appendChild(table._text2);

        tr = document.createElement("tr");
        tr.style.display = "none";
        table.appendChild(tr);
        tr.appendChild(document.createElement("td"));
        table._text3 = document.createElement("td");
        tr.appendChild(table._text3);

        tr = document.createElement("tr");
        tr.style.display = "none";
        table.appendChild(tr);
        table._text4 = document.createElement("td");
        tr.appendChild(table._text4);

        tr = document.createElement("tr");
        tr.style.display = "none";
        table.appendChild(tr);
        table._text5 = document.createElement("td");
        tr.appendChild(table._text5);
        setInterval(function () {
            var t = document._stiTooltip;
            var op = parseFloat(t.style.opacity);
            if ((t.cx > 0 && op < 1) || (t.cx < 0 && op > 0)) {
                op += t.cx;
                op = Math.min(1, Math.max(0, op));
                t.style.opacity = op;
            }
        }, 50);
    }

    var createChartTooltip = function () {
        var table = document.createElement("table");
        table.style.position = "absolute";
        table.style.opacity = "0";
        table.style.background = "white";
        table.style.padding = "5px";
        table.style.border = "1px solid #bebebe";
        table.style.fontFamily = "Arial";
        table.style.fontSize = "12px";
        table.style.color = "#111111";
        table.style.zIndex = "20000";
        table.style.pointerEvents = "none";
        document.body.appendChild(table);
        document._stiChartTooltip = table;
        var tr = document.createElement("tr");
        table.appendChild(tr);
        var td = document.createElement("td");
        td.style.verticalAlign = "center";
        td.style.width = "20px";
        table._round = document.createElement('div');
        table._round.style.width = "10px";
        table._round.style.height = "10px";
        td.appendChild(table._round);
        tr.appendChild(td);
        table._text1 = document.createElement("td");
        tr.appendChild(table._text1);
        tr = document.createElement("tr");
        table.appendChild(tr);
        table._text2 = document.createElement("td");
        table._text2.colSpan = 2;
        tr.appendChild(table._text2);

        tr = document.createElement("tr");
        tr.style.display = "none";
        table.appendChild(tr);
        table._text3 = document.createElement("td");
        tr.appendChild(table._text3);

        tr = document.createElement("tr");
        tr.style.display = "none";
        table.appendChild(tr);
        table._text4 = document.createElement("td");
        tr.appendChild(table._text4);

        tr = document.createElement("tr");
        tr.style.display = "none";
        table.appendChild(tr);
        table._text5 = document.createElement("td");
        tr.appendChild(table._text5);

        table.elements = new Object();
        setInterval(function () {
            var t = document._stiChartTooltip;
            var op = parseFloat(t.style.opacity);
            if ((t.cx > 0 && op < 1) || (t.cx < 0 && op > 0)) {
                op += t.cx;
                op = Math.min(1, Math.max(0, op));
                t.style.opacity = op;
            }
        }, 50);
    }

    var lightenDarkenColor = function (col, amt) {
        var usePound = false;
        if (col[0] == "#") {
            col = col.slice(1);
            usePound = true;
        }
        var num = parseInt(col, 16);
        var r = (num >> 16) + amt;
        if (r > 255) r = 255;
        else if (r < 0) r = 0;
        var b = ((num >> 8) & 0x00FF) + amt;
        if (b > 255) b = 255;
        else if (b < 0) b = 0;
        var g = (num & 0x0000FF) + amt;
        if (g > 255) g = 255;
        else if (g < 0) g = 0;
        return (usePound ? "#" : "") + String("000000" + (g | (b << 8) | (r << 16)).toString(16)).slice(-6);
    }

    var isTransparentFill = function (fill) {
        return fill != null && (fill.toLowerCase() == "transparent" || fill.toLowerCase() == "rgba(255, 255, 255, 0)");
    }

    var parseColor = function (color) {
        if (color) {
            var colorArr = color.split(",");
            if (colorArr.length == 4) {
                return "rgba(" + parseInt(colorArr[1]) + "," + parseInt(colorArr[2]) + "," + parseInt(colorArr[3]) + "," + parseInt(colorArr[0]) + ")";
            }
            else if (colorArr.length == 3) {
                return "rgb(" + parseInt(colorArr[0]) + "," + parseInt(colorArr[1]) + "," + parseInt(colorArr[2]) + ")";
            }
            else {
                return colorArr[0].toLowerCase();
            }
        }
        return "white";
    }

    var parseBorderStyle = function (borderStyle) {
        switch (borderStyle) {
            case "Solid":
                return "solid";

            case "Dash":
            case "DashDot":
            case "DashDotDot":
                return "dashed";

            case "Dot":
                return "dotted";

            case "Double":
                return "double";

            default: return "none";
        }
    }

    var applyTooltipBorderStyles = function (toolTip, border, cornerRadius) {
        if (border) {
            var bordArr = border.split(";");
            var bordSides = bordArr[0];
            var bordColor = parseColor(bordArr[1]);
            var bordSize = bordArr[2] || "1";
            var bordStyle = parseBorderStyle(bordArr[3] || "Solid");

            if (bordStyle != "none") {
                if (bordSides.indexOf("All") >= 0 || bordSides.indexOf("Left") >= 0) {
                    toolTip.style.borderLeft = bordSize + "px " + bordStyle + " " + bordColor;
                }
                if (bordSides.indexOf("All") >= 0 || bordSides.indexOf("Top") >= 0) {
                    toolTip.style.borderTop = bordSize + "px " + bordStyle + " " + bordColor;
                }
                if (bordSides.indexOf("All") >= 0 || bordSides.indexOf("Right") >= 0) {
                    toolTip.style.borderRight = bordSize + "px " + bordStyle + " " + bordColor;
                }
                if (bordSides.indexOf("All") >= 0 || bordSides.indexOf("Bottom") >= 0) {
                    toolTip.style.borderBottom = bordSize + "px " + bordStyle + " " + bordColor;
                }
            }
        }

        if (cornerRadius) {
            var radiusArr = cornerRadius.split(/[,;]+/);
            toolTip.style.borderRadius = radiusArr[0] + "px " + radiusArr[1] + "px " + radiusArr[2] + "px " + radiusArr[3] + "px";
        }
    }

    var applyTooltipBrushStyle = function (toolTip, brush, styleName) {
        if (brush) {
            var brushArr = brush.split(":");
            switch (brushArr[0]) {
                case "empty":
                    toolTip.style[styleName] = "transparent";
                    break;

                case "solid":
                case "glare":
                case "glass":
                case "hatch":
                    toolTip.style[styleName] = parseColor(brushArr[1]);
                    break;

                case "gradient":
                    toolTip.style[styleName] = styleName != "color"
                        ? "linear-gradient(" + parseInt(brushArr[3] || "0") + "deg, " + parseColor(brushArr[1]) + ", " + parseColor(brushArr[2]) + ")"
                        : parseColor(brushArr[1]);
                    break;
            }
        }
    }

    var applyTooltipStyles = function (toolTip, element) {
        var tBrush = element.getAttribute("_ttbrush");
        var tTextBrush = element.getAttribute("_tttextbrush");
        var tCornerRadius = element.getAttribute("_ttcornerradius");
        var tBorder = element.getAttribute("_ttborder");

        applyTooltipBorderStyles(toolTip, tBorder, tCornerRadius);
        applyTooltipBrushStyle(toolTip, tBrush, "background");
        applyTooltipBrushStyle(toolTip, tTextBrush, "color");
    }

    var addTooltip = function (el) {
        var t = document._stiTooltip;

        el.onmouseover = function (event) {
            if (this.getAttribute("notShowTooltip") == "true" || (!this.getAttribute("_text1") && !this.getAttribute("_text2")))
                return;

            var isCustomTooltip = this.getAttribute("isCustomTooltip") == "true";

            var text1 = this.getAttribute("_text1");

            applyTooltipStyles(t, event.target);

            if (isCustomTooltip && text1.toLowerCase().indexOf("<a ") >= 0) {
                t.style.pointerEvents = "auto";
                text1 = text1.replace(/<a /g, "<a target='_blank' ");
            }

            t.cx = 0.1;
            if (t._text1) t._text1.innerHTML = text1;
            if (t._text2) {
                t._text2.innerHTML = !isCustomTooltip ? this.getAttribute("_text2") : "";
                t._text2.parentElement.style.display = isCustomTooltip ? "none" : "";
            }
            if (t._round) t._round.parentElement.style.display = isCustomTooltip ? "none" : "";

            if (!isCustomTooltip) {
                for (var i = 3; i < 6; i++) {
                    var at = "_text" + i;
                    var text = this.getAttribute(at);
                    if (text && text.length > 0) {
                        t[at].parentNode.style.display = "";
                        t[at].innerHTML = text;
                    } else {
                        t[at].parentNode.style.display = "none";
                    }
                }
                t._round.style.background = this.getAttribute("_color");
            }
            else {
                clearTimeout(t.hiddenTimer);

                el.onmouseout = function (event) {
                    if (!isTransparentFill(this.style.fill))
                        this.style.fill = this.getAttribute("_color");

                    t.hiddenTimer = setTimeout(function () {
                        if (!t.isOver) t.cx = -0.2;
                    }, 500);
                }

                t.onmouseover = function () {
                    this.isOver = true;
                    clearTimeout(t.hiddenTimer);
                }

                t.onmouseout = function () {
                    this.isOver = false;
                    el.onmouseout();
                }

                t.onclick = function () {
                    this.onmouseout();
                }
            }

            if (!this.getAttribute("geomColor"))
                this.setAttribute("geomColor", this.style.fill);

            if (!isTransparentFill(this.style.fill))
                this.style.fill = lightenDarkenColor(this.getAttribute("_color"), -35);

            var cx = Math.max(event.pageX + 1 + t.offsetWidth - window.outerWidth + 10, 0);
            var cy = Math.max(event.pageY + 1 + t.offsetHeight - window.outerHeight + 10, 0);

            var browserHeight = window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight;
            var browserWidth = window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth;

            cx = Math.max(cx, event.pageX + 1 + t.offsetWidth - browserWidth);
            cy = Math.max(cy, event.pageY + 1 + t.offsetHeight - browserHeight);

            t.style.left = (event.pageX + 1 - cx) + "px";
            t.style.top = (event.pageY + 1 - cy) + "px";
        }

        el.onmouseout = function (event) {
            t.cx = -0.2;

            if (!isTransparentFill(this.style.fill))
                this.style.fill = this.getAttribute("geomColor") || this.getAttribute("_color");
        }
    }

    var inspect = function (element, animations) {

        if (!element) return;
        if (!document._stiChartTooltip) {
            createChartTooltip();
        }

        for (var i in element.childNodes) {
            var el = element.childNodes[i];
            if (el.attributes) {
                for (var j in el.attributes) {
                    if (el.attributes[j] && el.attributes[j].name) {
                        if (el.attributes[j].name.indexOf('_animation') >= 0) {
                            if (!el._animations) {
                                el._animations = [];
                                el._animations.push(JSON.parse(el.getAttribute(el.attributes[j].name)));
                                animations.push(el);
                            } else {
                                el._animations.push(JSON.parse(el.getAttribute(el.attributes[j].name)));
                            }
                        }
                        if (el.getAttribute("_ismap")) {
                            if (!document._stiTooltip) {
                                createTooltip();
                            }
                            addTooltip(el);
                        }
                    }
                }
            }
            inspect(el, animations);
        }
    }

    inspect(chart_, animations);
    var easeInOutQuad = function (t) { return t < .5 ? 2 * t * t : -1 + (4 - 2 * t) * t };
    var setScale = function (element, scaleX, scaleY, invertX, invertY) {
        var cx = !invertX ? element.bbox.x : element.bbox.x + element.bbox.width;
        var cy = !invertY ? element.bbox.y : element.bbox.y + element.bbox.height;
        var saclestr = scaleX + ',' + scaleY;
        var tx = -cx * (scaleX - 1);
        var ty = -cy * (scaleY - 1);
        var translatestr = tx + ',' + ty;
        element.setAttribute('transform', 'translate(' + translatestr + ') scale(' + saclestr + ')');
        element.setAttribute('opacity', '1');
    }
    var extractPoints = function (pointsStr) {
        var pointsA = pointsStr.split(" ");
        var result = [];
        for (var i = 0; i < pointsA.length - 1; i++) {
            var xy = pointsA[i].split(",");
            result.push({ x: parseFloat(xy[0]), y: parseFloat(xy[1]) });
        }
        return result;
    }


    var convertArcToCubicBezier = function (centerX, centerY, radius, startAngle1, sweepAngle1) {
        var startAngle = startAngle1 * Math.PI / 180;
        var sweepAngle = sweepAngle1 * Math.PI / 180;
        var endAngle = (startAngle1 + sweepAngle1) * Math.PI / 180;

        var x1 = centerX + radius * Math.cos(startAngle);
        var y1 = centerY + radius * Math.sin(startAngle);

        var x2 = centerX + radius * Math.cos(endAngle);
        var y2 = centerY + radius * Math.sin(endAngle);

        var l = radius * 4 / 3 * Math.tan(0.25 * sweepAngle);
        var aL = Math.atan(l / radius);
        var radL = radius / Math.cos(aL);

        aL += startAngle;
        var ax1 = centerX + radL * Math.cos(aL);
        var ay1 = centerY + radL * Math.sin(aL);

        aL = Math.atan(-l / radius);
        aL += endAngle;
        var ax2 = centerX + radL * Math.cos(aL);
        var ay2 = centerY + radL * Math.sin(aL);
        return [{ x: x1, y: y1 }, { x: ax1, y: ay1 }, { x: ax2, y: ay2 }, { x: x2, y: y2 }];
    }

    var round = function (value) {
        var value1 = parseInt(value);
        var rest = value - value1;
        return (rest > 0) ? value1 + 1 : value1;
    }

    var addArcPath = function (centerX, centerY, radius, startAngle, sweepAngle, dx, dy) {
        var result = "";
        var step = round(Math.abs(sweepAngle / 90));
        var stepAngle = sweepAngle / step;

        for (var indexStep = 0; indexStep < step; indexStep++) {
            var points = convertArcToCubicBezier(centerX - dx, centerY - dy, radius, startAngle, stepAngle);

            for (var index = 1; index < points.length - 1; index += 3) {
                if (index == 1)
                    result += "C" + (points[index].x + dx) + "," + (points[index].y + dy) + "," +
                        (points[index + 1].x + dx) + "," + (points[index + 1].y + dy) + "," +
                        (points[index + 2].x + dx) + "," + (points[index + 2].y + dy);
                else
                    result += "," + (points[index].x + dx) + "," + (points[index].y + dy) + "," +
                        (points[index + 1].x + dx) + "," + (points[index + 1].y + dy) + "," +
                        (points[index + 2].x + dx) + "," + (points[index + 2].y + dy);
            }
            startAngle += stepAngle;
        }

        return result;
    }

    var animatePie = function (dataStr, percent) {
        var data = JSON.parse(atob(dataStr));
        var result = "";
        var centerX = data.x + data.dx + data.width / 2;
        var centerY = data.y + data.dy + data.height / 2;
        var radius = data.width / 2;
        var radiusFrom = data.radiusFrom;
        radius = radiusFrom + (radius - radiusFrom) * percent;
        var startAngleFrom = data.startAngleFrom + (data.startAngle - data.startAngleFrom) * percent;
        var sweepAngleFrom = data.sweepAngleFrom + (data.sweepAngle - data.sweepAngleFrom) * percent;
        var startAngle = startAngleFrom * Math.PI / 180;

        var x1 = centerX + radius * Math.cos(startAngle);
        var y1 = centerY + radius * Math.sin(startAngle);

        result += "M" + centerX + "," + centerY;
        result += "L" + x1 + "," + y1;

        result += addArcPath(centerX, centerY, radius, startAngleFrom, sweepAngleFrom, data.dx, data.dy);

        result += "L" + centerX + "," + centerY;
        return result;
    }

    var animateDoughnut = function (dataStr, percent) {
        var data = JSON.parse(atob(dataStr));
        var result = "";
        var centerX = data.x + data.dx + data.width / 2;
        var centerY = data.y + data.dy + data.height / 2;
        var radius = data.width / 2;
        var radiusDt = data.widthDt / 2;
        var radiusFrom = data.radiusFrom;
        var radiusDtFrom = data.radiusDtFrom;
        radius = radiusFrom + (radius - radiusFrom) * percent;
        radiusDt = radiusDtFrom + (radiusDt - radiusDtFrom) * percent;
        var startAngleFrom = data.startAngleFrom + (data.startAngle - data.startAngleFrom) * percent;
        var sweepAngleFrom = data.sweepAngleFrom + (data.sweepAngle - data.sweepAngleFrom) * percent;
        var endAngleFrom = startAngleFrom + sweepAngleFrom;
        var startAngle = startAngleFrom * Math.PI / 180;
        var endAngle = (startAngleFrom + sweepAngleFrom) * Math.PI / 180;

        var x1 = centerX + radius * Math.cos(startAngle);
        var y1 = centerY + radius * Math.sin(startAngle);

        var xDt1 = centerX + radiusDt * Math.cos(startAngle);
        var yDt1 = centerY + radiusDt * Math.sin(startAngle);

        var xDt2 = centerX + radiusDt * Math.cos(endAngle);
        var yDt2 = centerY + radiusDt * Math.sin(endAngle);

        result += "M" + xDt1 + "," + yDt1;
        result += "L" + x1 + "," + y1;
        result += addArcPath(centerX, centerY, radius, startAngleFrom, sweepAngleFrom, data.dx, data.dy);
        result += "L" + xDt2 + "," + yDt2;
        result += addArcPath(centerX, centerY, radiusDt, endAngleFrom, -sweepAngleFrom, data.dx, data.dy);

        return result;
    }

    var animatePath = function (data, percent) {
        var result = "";
        while (data.length > 0) {
            result += data[0];
            var endIndex = data.substring(1).search(/[MLC]/) + 1;
            var els = data.substring(1, endIndex > 0 ? endIndex : data.length).split(/[, ]/);
            for (var i = 0; i < els.length; i++) {
                if (els[i] != "") {
                    var se = els[i].split(":");
                    result += (parseFloat(se[0]) + (parseFloat(se[1]) - parseFloat(se[0])) * percent);
                    if (i != els.length - 1) {
                        result += " ";
                    }
                }
            }
            data = endIndex > 0 ? data.substring(endIndex) : "";
        }
        return result;
    }

    var begin = new Date().getTime();
    var step = function (timestamp) {
        var finished = true;
        var now = new Date().getTime() - begin;
        for (var i in animations) {
            var an = animations[i];
            for (var k in an._animations) {
                var anim = an._animations[k];
                if (anim.begin <= now && anim.begin + anim.duration >= now) {
                    var percent = easeInOutQuad((now - anim.begin) / anim.duration);
                    for (var j in anim.actions) {
                        var ac = anim.actions[j];
                        var prefix = ac.length == 5 ? ac[4] : "";
                        if (ac[0] == "scaleCenter") {
                            an.bbox = an.getBBox();
                            setScale(an, (ac[1] + (ac[2] - ac[1]) * percent), (ac[3] + (ac[4] - ac[3]) * percent), ac[5], ac[6]);
                        } else if (ac[0] == "points") {
                            var pointsFrom = extractPoints(ac[1]);
                            var pointsTo = extractPoints(ac[2]);
                            var points = "";
                            for (var l in pointsFrom) {
                                points += (pointsFrom[l].x + (pointsTo[l].x - pointsFrom[l].x) * percent) + "," +
                                    (pointsFrom[l].y + (pointsTo[l].y - pointsFrom[l].y) * percent) + " ";
                            }
                            an.setAttribute(ac[0], points);
                        } else if (ac[0] == "value") {
                            var value = ac[1] + (ac[2] - ac[1]) * percent;
                            value = Math.round(value * Math.pow(10, ac[3])) / Math.pow(10, ac[3]);
                            an.textContent = value;
                        } else if (ac[0] == "translate") {
                            var from = ac[1].split(":");
                            var to = ac[2].split(":");
                            an.setAttribute("transform", "translate(" + (parseFloat(from[0]) + (parseFloat(to[0]) - parseFloat(from[0])) * percent) + " " +
                                (parseFloat(from[1]) + (parseFloat(to[1]) - parseFloat(from[1])) * percent) + ")" + ac[3]);
                        } else if (ac[0] == "path") {
                            an.setAttribute("d", animatePath(ac[1], percent));
                        } else if (ac[0] == "pie") {
                            an.setAttribute("d", animatePie(ac[1], percent));
                        } else if (ac[0] == "doughnut") {
                            an.setAttribute("d", animateDoughnut(ac[1], percent));
                        } else {
                            an.setAttribute(ac[0], prefix + (ac[1] + (ac[2] - ac[1]) * percent) + ac[3]);
                        }
                    }
                    finished = false;
                } else if (anim.begin + anim.duration < now) {
                    for (var j in anim.actions) {
                        var ac = anim.actions[j];
                        var prefix = ac.length == 5 ? ac[4] : "";
                        if (ac[0] == "scaleCenter") {
                            setScale(an, ac[2], ac[4], ac[5], ac[6]);
                        } if (ac[0] == "points") {
                            var pointsTo = extractPoints(ac[2]);
                            var points = "";
                            for (var l in pointsTo) {
                                points += pointsTo[l].x + "," + pointsTo[l].y + " ";
                            }
                            an.setAttribute(ac[0], points);
                        } else if (ac[0] == "value") {
                            an.textContent = ac[4] || ac[2];
                        } else if (ac[0] == "translate") {
                            var to = ac[2].split(":");
                            an.setAttribute("transform", "translate(" + to[0] + " " + to[1] + ")" + ac[3]);
                        } else if (ac[0] == "path") {
                            an.setAttribute("d", animatePath(ac[1], 1));
                        } else if (ac[0] == "pie") {
                            an.setAttribute("d", animatePie(ac[1], 1));
                        } else if (ac[0] == "doughnut") {
                            an.setAttribute("d", animateDoughnut(ac[1], 1));
                        } else {
                            an.setAttribute(ac[0], prefix + ac[2] + ac[3]);
                        }
                    }
                } else if (anim.begin > now) {
                    finished = false;
                }
            }
        }
        if (!finished) {
            requestAnimationFrame(step);
        }
    }
    requestAnimationFrame(step);
}