using System;
using System.Configuration;


public class ConstCorreo
{
    public static string CORREOBOLETA = ConfigurationManager.AppSettings["CORREOBOLETA"];
    public static string CLAVE_BOLETA = ConfigurationManager.AppSettings["CLAVE_BOLETA"];
    public static string CORREO = ConfigurationManager.AppSettings["CORREO"];
    public static string CLAVE_CORREO = ConfigurationManager.AppSettings["CLAVE_CORREO"];
    public static string CC_CORREO = ConfigurationManager.AppSettings["CC_CORREO"];
    public static string HOST_OUTLOOK = ConfigurationManager.AppSettings["HOST_OUTLOOK"];
    public static string HOST_GMAIL = ConfigurationManager.AppSettings["HOST_GMAIL"];
    public static string HOST_OFFICE = ConfigurationManager.AppSettings["HOST_OFFICE"];
    public static int PUERTO = Convert.ToInt32(ConfigurationManager.AppSettings["PUERTO"]);
}

public class Conexion
{
    public static string connetionString = ConfigurationManager.ConnectionStrings["DefaulConnectionRoe"].ConnectionString;
}

public class Encrypt
{
    public static string ED_KEY = ConfigurationManager.AppSettings["ENDECRY"];
}

public static class ConstantesGlobales
{
    //tb_Area
    public static string ninguno = "11";

    //tabla tb_Cargo
    public static string NI = "0"; //cargo NINGUNO
    public static string GP = "25"; //cargo gerente producto
    public static string GPJ = "26";//cargo gerente producto junior
    public static string VEN = "42";//VENDEDOR
    public static string SUP = "41";// SUPERVISOR
    public static string REP = "37";// REPRESENTANTE
    public static string GM = "24";//GERENTE DE MARKETING
    //tabla tb_Estado
    public static string estadoCesado = "2";//estado cesado
    public static string estadoActivo = "1";//estado activo
    public static string estadoInactivo = "4";//estado Inactivo
    public static string estadoRegistrado = "7";//estado Registrado
    public static string estadoModificado = "11";//estado Modificado
    public static string estadoAnulado = "14";//estado Anulado
    public static string estadoPreApro = "8";//estado Anulado
    public static string estadoAprobado = "9";//estado Anulado
    public static string estadoRechazado = "12";//estado Anulado
    public static string estadoAtendido = "10";
    public static string estadoNoAtendido = "13";
    public static string estadoDescontinuado = "6";
    public static string estadoLiquidado = "15";
    public static string estadoDParcial = "16";
    public static string estadoDTotal = "17";
    public static string estadoReembolso = "18";
    public static string estadoAceptada = "21";
    public static string estadoCancelada = "22";
    public static string estadoEnGiro = "23";//Estado Solicitud de Ventas o Mkt 


    //tabla tb_Moneda
    public static string monedaSol = "1";//moneda soles
    public static string monedaDol = "2";//moneda soles

    //tabla tb_Roles
    public static string rolInicio = "000000";
    public static string rolBienvenida = "000006";
    public static string rolTodos = "ALL";

    //tabla tb_TipRol
    public static string tipRol_Ninguno = "00";
    public static string tipRol_Total = "01";
    public static string tipRol_Area = "02";
    public static string tipRol_Control = "03";
    public static string tipRol_Vista = "04";

    //tabla tb_TipMenu
    public static string tMenu_Plantilla = "01";
    public static string tMenu_Menu = "02";
    public static string tMenu_Sub = "03";
    public static string tMenu_Vista = "04";
    public static string tMenu_LinPar = "05";

    //tabla tb_Parametro
    public static string Com_SolGas_Reg_Cas_01 = "00000001";
    public static string Com_SolGas_Reg_Cas_02 = "00000002";
    public static string Com_SolGas_Reg_Resp = "00000003";
    public static string Com_PtoGas_Tipo_Cas_01 = "00000004";//parametro tipo ventas
    public static string Com_PtoGas_Tipo_Cas_02 = "00000005";//parametro tipo mkt
    public static string Com_PtoGas_Tipo_Cas_03 = "00000010";//parametro tipo fuera del plan
    public static string Com_PtoGas_Tipo_Pres_01 = "00000006";//detalle parametro ventas
    public static string Com_PtoGas_Tipo_Pres_02 = "00000009";//detalle parametro mkt
    public static string Com_PtoGas_Tipo_Pres_03 = "00000011";//detalle parametro fuera del plan
    public static string Com_Usu_Pre_Cas_01 = "00000006";
    public static string Com_Usu_Pre_Cas_02 = "00000008";
    public static string Com_Usu_Pre_Cas_03 = "00000009";
    public static string Com_Usu_Pre_Cas_04 = "00000011";
    public static string Com_Lis_Ven = "00000007";
    public static string Com_Lis_Apr = "00000008";
    public static string Com_Apr_Let = "00000013";//Lista de Aprobadores de Letras

    //tabla tb_TipPag
    public static string tipPag_tran = "1";
    public static string tipPag_tarCre = "2";

    //tabla tb_TipSol
    public static string tipSol_Ren = "2";

    //tabla tb_NivApr
    public static string CERO = "0";
    public static string PRIMERO = "1";
    public static string SEGUNDO = "2";
    public static string TERCERO = "3";

    //tabla tb_tipPres
    public static string plan_Mark = "002";
    public static string plan_Trab = "003";
    public static string plan_Fuera = "001";

    //tabla tb_tipActGas
    public static string gasto_Medico = "1";
    public static string gasto_Staff = "2";
    public static string gasto_Varios = "3";

    //modulo de solicitud de gasto
    public static string mod_ventas = "V";
    public static string mod_marketing = "M";

    //tabla Tipo de Gasto 
    public static string tipGas_Congreso = "3";

    //tabla tb_ConGas
    public static string conGas_Congreso = "34";
    public static string tipGas_Actividad = "20";

    //tabla tb_Zona
    public static string zona_ninguno = "0";
    public static string zona_Lima = "29";

    //Valores IdUser Administradores
    public static string[] administrator = new string[] { "kaylas", "hpadilla", "mlanegra", "mdaguila", "wjave" };
    public static string[] administratorMkt = new string[] { "dramirez", "mmardini"};

    //Especialidad explicita
    public static string Especialidad1 = "ODONTOLOGIA";
    public static string Especialidad2 = "OBSTETRICIA";

    //tabla
    public static string TBFORUSU = "tb_For_Usu";//GERENTE DE MARKETING
    public static string TBFORMU = "tb_Formulario";//GERENTE DE MARKETING

    //tabla tb_TipDocRRHH
    public static string tipDocPolitica = "0000001";
    public static string tipDocReglamento = "0000002";
    public static string tipDocSeguridad = "0000003";
    public static string tipDocCovid = "0000004";

    //tabla tb_subtipoRRHH
    public static string subTipoVacaciones = "1"; //vacaciones
    public static string tipoVacaciones = "1";//tipo vacaciones
    public static string tipoDescansos = "2";//tipo descansos médicos
    public static string tipoLicencias = "3";//tipo licencias

}

public class Sessiones
{
    public static string menu = "MENU_USER";
    public static string cumple = "CUMPLE_USER";
    public static string enlace = "ENLACE_USER";
    public static string empleado = "EMPLEADOMODEL";
    public static string formulario = "FORMULARIOMODEL";
}