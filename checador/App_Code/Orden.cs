using System;
using System.Collections.Generic;
//using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Orden
/// </summary>
public class Orden
{
    public Orden()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    private static string sCampoOrden;
    public static string CampoOrden
    {
        set
        {
            sCampoOrden = value;
        }
        get
        {
            return sCampoOrden;
        }
    }

    private static string sAscDes;
    public static string AscDes
    {
        set
        {
            sAscDes = value;
        }
        get
        {
            return sAscDes;
        }
    }
    private static string sDirectorio;
    public static string Directorio
    {
        set
        {
            sDirectorio = value;
        }
        get
        {
            return sDirectorio;
        }
    }
    private static string sAccion;
    public static string Accion
    {
        set
        {
            sAccion = value;
        }
        get
        {
            return sAccion;
        }
    }
}