//solo para eliminar
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
    $("div #success").fadeOut(3000).fadeIn(3000).fadeOut(2500).fadeIn(2500).fadeOut(1500);
    $("div #danger").fadeOut(3000).fadeIn(3000).fadeOut(2500).fadeIn(2500).fadeOut(1500);
    $("div #info").fadeOut(3000).fadeIn(3000).fadeOut(2500).fadeIn(2500).fadeOut(1500);
    $("div #warning").fadeOut(3000).fadeIn(3000).fadeOut(2500).fadeIn(2500).fadeOut(1500);
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
}

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
