using System.Web;
using System.Web.Optimization;

namespace PortalRoemmers
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            //CSS///////////////////////////////////////////////////////////
            bundles.Add(new StyleBundle("~/Content/Inicio_CCS").Include(
                              "~/Content/bootstrap/dist/css/bootstrap.min.css",
                              "~/Content/plugins/bower_components/sidebar-nav/dist/sidebar-nav.min.css",
                              "~/Content/css/style.css",
                              "~/Content/css/animate.css",
                              "~/Content/css/colors/megna-dark.css"));
            //tabla responsive
            bundles.Add(new StyleBundle("~/Content/tablaResponsive_CSS").Include("~/Content/plugins/bower_components/tablesaw-master/dist/tablesaw.css"));
            //select con busqueda
            bundles.Add(new StyleBundle("~/Content/selectBusqueda_CSS").Include("~/Content/plugins/bower_components/custom-select/custom-select.css"));
            //autocompletado
            bundles.Add(new StyleBundle("~/Content/autocompletado_CSS").Include("~/Content/css/jquery-ui.css"));
            //datepicker
            bundles.Add(new StyleBundle("~/Content/datepicker_CSS").Include("~/Content/plugins/bower_components/bootstrap-datepicker/bootstrap-datepicker.min.css"));
            //alerts CSS 
            bundles.Add(new StyleBundle("~/Content/sweetalert_CSS").Include("~/Content/plugins/bower_components/sweetalert/sweetalert.css"));
            //Accordion Wizard 
            bundles.Add(new StyleBundle("~/Content/AccordionWizard_CSS").Include("~/Content/plugins/bower_components/jquery-wizard-master/css/wizard.css"));
            //tabla export
            bundles.Add(new StyleBundle("~/Content/tablaExport_CSS").Include(
                                        "~/Content/plugins/bower_components/datatables/jquery.dataTables.min.css",
                                        //"~/Content/plugins/bower_components/datatables/media/css/jquery.dataTables.min.css",
                                        "~/Content/plugins/bower_components/datatables/extenos/buttons.dataTables.min.css"));
            //galeria
            bundles.Add(new StyleBundle("~/Content/Galeria_CSS").Include(
                "~/Content/plugins/bower_components/gallery/css/animated-masonry-gallery.css",
                "~/Content/plugins/bower_components/fancybox/ekko-lightbox.min.css"));
            //magnific-popup
            bundles.Add(new StyleBundle("~/Content/Magnific_CSS").Include("~/Content/plugins/bower_components/Magnific-Popup-master/dist/magnific-popup.css"));
            //SUMMERNOTES edicion
            bundles.Add(new StyleBundle("~/Content/summernote_CSS").Include("~/Content/plugins/bower_components/summernote/dist/summernote.css"));
            //DROPZONE
            bundles.Add(new StyleBundle("~/Content/dropzone_CSS").Include("~/Content/plugins/bower_components/dropzone-master/dist/dropzone.css"));
            //Bootstrap
            bundles.Add(new StyleBundle("~/Content/Bootstrap_CSS").Include("~/Content/plugins/bower_components/bootstrap-table/dist/bootstrap-table.min.css"));
            //Daterange picker plugins css
            bundles.Add(new StyleBundle("~/Content/Daterange_CSS").Include(
                "~/Content/plugins/bower_components/timepicker/bootstrap-timepicker.min.css",
                "~/Content/plugins/bower_components/bootstrap-daterangepicker/daterangepicker.css"));

            //JS///////////////////////////////////////////////////////////
            bundles.Add(new ScriptBundle("~/Bundles/Inicio_JS").Include(
                        // "~/Scripts/js/jquery-1.10.2.js",
                         "~/Content/plugins/bower_components/jquery/dist/jquery.min.js",
                         "~/Content/bootstrap/dist/js/bootstrap.min.js",
                         "~/Content/plugins/bower_components/sidebar-nav/dist/sidebar-nav.min.js",
                         "~/Scripts/js/jquery.slimscroll.js",
                         "~/Scripts/js/waves.js",
                         "~/Scripts/js/custom.min.js"));
            //tabla responsive
            bundles.Add(new ScriptBundle("~/Bundles/tablaResponsive_JS").Include(
               "~/Content/plugins/bower_components/tablesaw-master/dist/tablesaw.js",
               "~/Content/plugins/bower_components/tablesaw-master/dist/tablesaw-init.js" ));
            //select con busqueda
            bundles.Add(new ScriptBundle("~/Bundles/selectBusqueda_JS").Include("~/Content/plugins/bower_components/custom-select/custom-select.min.js"));
            //autocompletado
            bundles.Add(new ScriptBundle("~/Bundles/autocompletado_JS").Include("~/Scripts/js/jquery-ui-1.12.1.js"));
            //datepicker
            bundles.Add(new ScriptBundle("~/Bundles/datepicker_JS").Include("~/Content/plugins/bower_components/bootstrap-datepicker/bootstrap-datepicker.min.js"));
            //Sweet - Alert
            bundles.Add(new ScriptBundle("~/Bundles/sweetalert_JS").Include("~/Content/plugins/bower_components/sweetalert/sweetalert.min.js"));
            //Accordion Wizard 
            bundles.Add(new ScriptBundle("~/Bundles/AccordionWizard_JS").Include("~/Content/plugins/bower_components/jquery-wizard-master/dist/jquery-wizard.min.js"));
            //bloqueo 
            bundles.Add(new ScriptBundle("~/Bundles/bloqueo_JS").Include("~/Content/plugins/bower_components/blockUI/jquery.blockUI.js"));
            //tabla export
            bundles.Add(new ScriptBundle("~/Bundles/tablaExport_JS").Include(
                "~/Content/plugins/bower_components/datatables/jquery.dataTables.min.js",
                "~/Content/plugins/bower_components/datatables/extenos/dataTables.buttons.min.js",
                "~/Content/plugins/bower_components/datatables/extenos/buttons.flash.min.js",
                "~/Content/plugins/bower_components/datatables/extenos/jszip.min.js",
                "~/Content/plugins/bower_components/datatables/extenos/pdfmake.min.js",
                "~/Content/plugins/bower_components/datatables/extenos/vfs_fonts.js",
                "~/Content/plugins/bower_components/datatables/extenos/buttons.html5.min.js",
                "~/Content/plugins/bower_components/datatables/extenos/buttons.print.min.js"));
            //galeria
            bundles.Add(new ScriptBundle("~/Bundles/Galeria_JS").Include("~/Content/plugins/bower_components/gallery/js/jquery.isotope.min.js",
                "~/Content/plugins/bower_components/fancybox/ekko-lightbox.min.js"));
            //Magnific popup
            bundles.Add(new ScriptBundle("~/Bundles/Magnific_JS").Include(
                "~/Content/plugins/bower_components/Magnific-Popup-master/dist/jquery.magnific-popup.min.js",
                "~/Content/plugins/bower_components/Magnific-Popup-master/dist/jquery.magnific-popup-init.js"));
            //mascara de inputs
            bundles.Add(new ScriptBundle("~/Bundles/mascara_JS").Include("~/Scripts/js/mask.js"));
            //SUMMERNOTES edicion
            bundles.Add(new ScriptBundle("~/Bundles/summernote_JS").Include("~/Content/plugins/bower_components/summernote/dist/summernote.min.js"));
            //DROPZONE
            bundles.Add(new ScriptBundle("~/Bundles/dropzone_JS").Include("~/Content/plugins/bower_components/dropzone-master/dist/dropzone.js"));
            //codebar scaner
            bundles.Add(new ScriptBundle("~/Bundles/scannerCodeBar").Include("~/Scripts/js/jquery-code-scanner.js"));
            //Bootstrap
            bundles.Add(new ScriptBundle("~/Bundles/Bootstrap_JS").Include(
                "~/Content/plugins/bower_components/bootstrap-table/dist/bootstrap-table.min.js",
                "~/Content/plugins/bower_components/bootstrap-table/dist/bootstrap-table.ints.js"));
            //Date range Plugin JavaScript
            bundles.Add(new ScriptBundle("~/Bundles/Daterange_JS").Include(
                "~/Content/plugins/bower_components/moment/moment.js",
                "~/Content/plugins/bower_components/timepicker/bootstrap-timepicker.min.js",
                "~/Content/plugins/bower_components/bootstrap-daterangepicker/daterangepicker.js"));

        }
    }
}
