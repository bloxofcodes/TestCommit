using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QReports.Controllers
{
    public class PmWorkOrderController : Controller
    {
        SAAEntities saaentities = new SAAEntities();
        // GET: PmWorkOrder
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult PmWorkOrderList()
        {

            List<string> woType = new List<string>();
            List<string> asstGrp = new List<string>();
            List<string> woStat = new List<string>();
            List<string> locsGrp = new List<string>();
            List<string> asstItems = new List<string>();

            List<string> asstTypes = new List<string>();
            List<string> tech = new List<string>();
            List<string> reportBy = new List<string>();

            //Query Work Order Types
            var queryWoType = (from wotype in saaentities.T_TYPE_TRAV
                               where wotype.DES_TYPE_TRAV != ""
                               select new
                               {
                                   WOTYPE = wotype.DES_TYPE_TRAV
                               }).AsEnumerable()
                        .Select(c => new {
                            WOTYPE = c.WOTYPE
                        }).ToList();

            //Query Asset Groups
            var queryAssGrp = (from assgrp in saaentities.T_FAMILLE_UI
                               where assgrp.CODE_FAM != ""
                               select new
                               {
                                   ASSGRP = assgrp.CODE_FAM
                               }).AsEnumerable()
                               .Select(c => new
                               {
                                   ASSGRP = c.ASSGRP
                               }).ToList();
            //Query Work Order Status
            var queryWoStat = (from wostat in saaentities.T_ETAT_BT
                               where wostat.DES_ETAT_BT != null && wostat.DES_ETAT_BT != ""
                               select new
                               {
                                   WOSTAT = wostat.DES_ETAT_BT
                               }).AsEnumerable()
                               .Select(c => new
                               {
                                   WOSTAT = c.WOSTAT
                               }).ToList();
            //Query Locations
            var queryLocs = (from locs in saaentities.T_LIEU
                             where locs.LIBELLE_LIEU != null && locs.LIBELLE_LIEU != ""
                             select new
                             {
                                 LOCS = locs.LIBELLE_LIEU
                             }).AsEnumerable()
                             .Select(c => new
                             {
                                 LOCS = c.LOCS
                             }).ToList();

            //Query Assets
            var queryAssts = (from assts in saaentities.T_UI
                              where assts.DESIGNATION_UI != null && assts.DESIGNATION_UI != ""
                              select new
                              {
                                  ASSTS = assts.DESIGNATION_UI
                              }).AsEnumerable()
                              .Select(c => new
                              {
                                  ASSTS = c.ASSTS
                              }).ToList();

            //Query AssetType
            var queryAsstTyp = (from assttype in saaentities.T_TYPE_UI
                                where assttype.LIBELLE_TUI != null && assttype.LIBELLE_TUI != ""
                                select new
                                {
                                    ASSTTYPE = assttype.LIBELLE_TUI
                                }).AsEnumerable()
                                .Select(c => new
                                {
                                    ASSTTYPE = c.ASSTTYPE
                                }).ToList();
            //Query Technician
            var queryTech = (from techrep in saaentities.T_INTERV
                             where techrep.PRENOM_INTERV != null && techrep.PRENOM_INTERV != ""
                             select new
                             {
                                 TECHREP = techrep.PRENOM_INTERV
                             }).AsEnumerable().
                             Select(c => new
                             {
                                 TECHREP = c.TECHREP
                             }).ToList();

            //Query ReportBy
            var queryRepoBy = (from repoby in saaentities.T_DEMANDEUR
                               where repoby.NOM_DEMANDEUR != null && repoby.NOM_DEMANDEUR != ""
                               select new
                               {
                                   REPOBY = repoby.NOM_DEMANDEUR
                               }).AsEnumerable()
                               .Select(c => new
                               {
                                   REPOBY = c.REPOBY
                               }).ToList();



            foreach (var item in queryAssGrp)
                asstGrp.Add(item.ASSGRP);

            foreach (var wtitem in queryWoType)
                woType.Add(wtitem.WOTYPE);

            foreach (var wsitem in queryWoStat)
                woStat.Add(wsitem.WOSTAT);

            foreach (var locs in queryLocs)
                locsGrp.Add(locs.LOCS);

            foreach (var assts in queryAssts)
                asstItems.Add(assts.ASSTS);
            foreach (var assttype in queryAsstTyp)
                asstTypes.Add(assttype.ASSTTYPE);

            foreach (var techrep in queryTech)
                tech.Add(techrep.TECHREP);

            foreach (var repoby in queryRepoBy)
                reportBy.Add(repoby.REPOBY);


            ViewBag.wotype = woType;
            ViewBag.assgrp = asstGrp;
            ViewBag.wostat = woStat;
            ViewBag.locs = locsGrp;
            ViewBag.assts = asstItems;
            ViewBag.assttype = asstTypes;
            ViewBag.techrep = tech;
            ViewBag.repoby = reportBy;

            return View();
        }
    }
}