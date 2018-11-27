using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EasyFreight.DAL;
using EasyFreight.Models;
using Rotativa;

namespace EasyFreight.Controllers
{
	public class DocsToPrintController : Controller
	{
		//
		// GET: /DocsToPrint/
		public ActionResult ConcessionLetterEn(int operationId)
		{
			return View();
		}

		[AllowAnonymous]
		public ActionResult ConcessionLetterAr(int operationId)
		{
			var concLetter = OperationHelper.GetConcessionLetter(operationId, "ar");
			return View(concLetter);
		}

		public ActionResult PrintConcessionLetter(int operationId, string langCode)
		{
			return new ActionAsPdf("ConcessionLetterAr", new { operationId = operationId, name = "Giorgio" }) { FileName = "ConcessionLetterAr.pdf" };
		}

		[AllowAnonymous]
		public ActionResult ConcessionLetterAirAr(int operationId)
		{
			var concLetter = OperationHelper.GetConcessionLetter(operationId, "ar");
			return View(concLetter);
		}

		public ActionResult PrintConcessionLetterAir(int operationId, string langCode)
		{
			return new ActionAsPdf("ConcessionLetterAirAr", new { operationId = operationId, name = "Giorgio" }) { FileName = "ConcessionLetterAr.pdf" };
		}

		[AllowAnonymous]
		public ActionResult DeliveryNoteAr(int hbId)
		{
			var concLetter = HouseBillHelper.GetDeliveryNoteInfo(hbId, "ar");
			return View(concLetter);
		}


		public ActionResult PrintDeliveryOrder(int houseBillId, string langCode)
		{
			return new ActionAsPdf("DeliveryNoteAr", new { hbId = houseBillId, name = "Giorgio" }) { FileName = "ConcessionLetterAr.pdf" };
		}


		[AllowAnonymous]
		public ActionResult PreAlertLetter(int houseBillId,  int isNotifier=0)
		{
			
			HouseBillView hbill = HouseBillHelper.GetHBView(houseBillId);
			int operationID = hbill.OperationId;
			OperationView operation = OperationHelper.GetOne(operationID);
			ViewBag.houseBL = hbill.HouseBL; 
			ViewBag.isNotifie = isNotifier;
			return View(hbill);//operation
		}

		public ActionResult PrintPreAlertLetter(int houseBillId, int isNotifier)
		{
			return new ActionAsPdf("PreAlertLetter", new { houseBillId = houseBillId, isNotifier = isNotifier  }) { FileName = "PreAlertLetter.pdf" };
		}


	}
}