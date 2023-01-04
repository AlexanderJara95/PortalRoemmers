﻿//solo para eliminar
$(document).on("click", ".elimfila", function () {
    var parent = $(this).parents().get(0);
    $(parent).remove();
});
//cada vez que cargue el dom
$(document).ready(function () {
    $("#email").change(function () {
        var total = $("#email").val().indexOf("@");
        $("#username").val($("#email").val().substr(0, total));
    });
    $("div #success").fadeOut(2000).fadeIn(2000).fadeOut(1500).fadeIn(1500).fadeOut(500);
    $("div #danger").fadeOut(2000).fadeIn(2000).fadeOut(1500).fadeIn(1500).fadeOut(500);
    $("div #info").fadeOut(2000).fadeIn(2000).fadeOut(1500).fadeIn(1500).fadeOut(500);
    $("div #warning").fadeOut(2000).fadeIn(2000).fadeOut(1500).fadeIn(1500).fadeOut(500);
});

//funciones
function Hidden(input, table, columns, delimiter) {
    if ($('#' + table + ' tbody tr').length > 0) {//verifico que la grilla no este vacia

        var colms = columns.split("|");//pongo comas al texto que envie
        $("#" + input).val("");//limpio porsiacaso el input
        var aux = "";
        var req = "";

        $('#' + table + ' tbody tr').each(function (index) {//recorro la tabla 
            req = $("#" + input).val();
            if (req != "") {
                req += delimiter;
            }
            $.each(colms, function (id, elem) {//recorro lo que ya esta en comas
                if ($('#' + table + ' tbody tr').eq(index).find("td").eq(parseInt(elem)).find("input").length) {
                    aux = $('#' + input + ' tbody tr').eq(index).find("td").eq(parseInt(elem)).find("input").eq(0).val();
                    if (aux.length < 1) {
                        req = "";
                        return false;
                    } else {
                        req += aux + ";";
                    }
                } else {
                    req += $('#' + table + ' tbody tr').eq(index).find("td").eq(parseInt(elem)).html() + ";";
                }
            });
            $("#" + input).val(req.substring(0, req.length - 1));//agrego el valor al input
        });
    }
    console.log($("#" + input).val());
}

function Search(table,buscar,post) {
    var row = "";

    var b = $('#' + buscar).val();

    $("#" + table + " > tbody:last").empty();
    block();

    $.post(post, { buscar: b }, function (data, status) {
        if (status == "success") {
            if (data == "") {
                row = "<tr>";
                row += "<td colspan='2' class='text-center'>" + "No encontrado" + "</td>";
                row += "</tr>";
                $("#" + table + " > tbody:last").append(row);
            }
            else {
                $.each(data, function (i, elem) {
                    row = "<tr>";
                    row += "<td class='text-center'>" + elem.codigo + "</td>";
                    row += "<td class='text-center'>" + elem.nombre + "</td>";
                    row += "<td class='hide'>" + elem.descripcion + "</td>";
                    row += "</tr>";
                    $("#" + table + " > tbody:last").append(row);
                });
            }
            unblock();
        }
    });
}

//----Nuevo cambia el dato de la tabla
function SearchDetalle(table, buscar, post) {
    var row = "";

    var b = $('#' + buscar).val();

    $("#" + table + " > tbody:last").empty();
    block();

    $.post(post, { buscar: b }, function (data, status) {
        if (status == "success") {
            if (data == "") {
                row = "<tr>";
                row += "<td colspan='2' class='text-center'>" + "No encontrado" + "</td>";
                row += "</tr>";
                $("#" + table + " > tbody:last").append(row);
            }
            else {
                $.each(data, function (i, elem) {
                    row = "<tr>";
                    row += "<td class='text-center'>" + elem.descripcion + "</td>";
                    row += "<td class='text-center'>" + elem.nombre + "</td>";
                    row += "<td class='hide'>" + elem.codigo + "</td>";
                    row += "</tr>";
                    $("#" + table + " > tbody:last").append(row);
                });
            }
            unblock();
        }
    });
}
/**
 *    <section id="wrapper" class="block1">
 *    </section>
 */
function block() {
    $('section.block1').block({
        message: '<h4><img src="../../Content/plugins/images/busy.gif" /> Sólo un momento...</h4>'
        , css: {
            border: '1px solid #fff'
        }
    });
}
function unblock() {
    $('section.block1').unblock();
}
//Nuevo 2020 - Buscar detalle de 4 datos
function BusquedaDetalle(table, buscar, post) {
    var row = "";

    var b = $('#' + buscar).val();

    $("#" + table + " > tbody:last").empty();
    block();

    $.post(post, { buscar: b }, function (data, status) {
        if (status == "success") {
            if (data == "") {
                row = "<tr>";
                row += "<td colspan='2' class='text-center'>" + "No encontrado" + "</td>";
                row += "</tr>";
                $("#" + table + " > tbody:last").append(row);
            }
            else {
                $.each(data, function (i, elem) {
                    row = "<tr>";
                    row += "<td class='text-center'>" + elem.nom1 + "</td>";
                    row += "<td class='text-center'>" + elem.nom2 + "</td>";
                    row += "<td class='hide'>" + elem.cod1 + "</td>";
                    row += "<td class='hide'>" + elem.cod2 + "</td>";
                    row += "</tr>";
                    $("#" + table + " > tbody:last").append(row);
                });
            }
            unblock();
        }
    });
}