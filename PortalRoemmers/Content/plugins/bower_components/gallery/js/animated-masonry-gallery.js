$(window).load(function () {
/*Modificado por PAD*/ 
var size = 1;
var button_class = "gallery-header-center-right-links-current";
var normal_size_class = "gallery-content-center-normal";
var full_size_class = "gallery-content-center-full";
var $container = $('#gallery-content-center');
$container.isotope({itemSelector : 'img'});
function check_button(){
    $('.gallery-header-center-right-links').removeClass(button_class);
    $("#filter-all").addClass(button_class);
    $("#gallery-header-center-left-title").html('All Galleries');
}
function check_size(){
	$("#gallery-content-center").removeClass(normal_size_class).removeClass(full_size_class);
	if(size==0){
		$("#gallery-content-center").addClass(normal_size_class); 
		$("#gallery-header-center-left-icon").html('<span class="iconb" data-icon="&#xe23a;"></span>');
		}
	if(size==1){
		$("#gallery-content-center").addClass(full_size_class); 
		$("#gallery-header-center-left-icon").html('<span class="iconb" data-icon="&#xe23b;"></span>');
		}
	$container.isotope({itemSelector : 'img'});
}
    $("#filter-all").click(function () {
        $container.isotope({ filter: '.all' });
        $('.gallery-header-center-right-links').removeClass(button_class);
        $("#filter-all").addClass(button_class);
        $("#gallery-header-center-left-title").html('All Galleries');
    });
    $.post('/Home/obtenerTipGal', function (data, status) {
        $.each(data, function (i, item) {
            $("#" + item.filTipGal).click(function () {
                $container.isotope({ filter: item.classTipGal });
                $('.gallery-header-center-right-links').removeClass(button_class);
                $("#" + item.filTipGal).addClass(button_class);
                $("#gallery-header-center-left-title").html(item.classesTipGal);
            });
        });
    });
    $("#gallery-header-center-left-icon").click(function () {
        if (size == 0) { size = 1; }
        else if (size == 1){ size = 0; }
        check_size();
    });
    check_button();
    check_size();
});