using System;
using System.Collections.Generic;
using System.Linq;
using Cw13.DTOs;
using Cw13.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cw13.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly BakeryContext _bakeryContext;

        public OrderController(BakeryContext context)
        {
            this._bakeryContext = context;
        }

        [HttpGet]
        public IActionResult GetZamowienia(GetDTO getDTO)
        {
            List<Zamowienie> orders = null;
            List<ZamowienieDTO> ordersDTO = new List<ZamowienieDTO>();
            
            if (getDTO.nazwisko != null)
            {
                Klient client = _bakeryContext.Klienci.FirstOrDefault(w => w.Nazwisko == getDTO.nazwisko);
                if (client != null)
                    orders = _bakeryContext.Zamowienia.Where(z => z.IdKlient == client.IdKlient).ToList();
                else
                    orders = _bakeryContext.Zamowienia.ToList();
            }
            else
            {
                orders = _bakeryContext.Zamowienia.ToList();
            }

            if (orders == null)
                return NotFound("Nie znaleziono zamowien");


            foreach (Zamowienie z in orders)
            {
                ZamowienieDTO orderDTO = new ZamowienieDTO();
                orderDTO.dataPrzyjecia = z.DataPrzyjecia.ToString();
                orderDTO.uwagi = z.Uwagi;

                List<WyrobDTO> productsDTO = new List<WyrobDTO>();


                foreach (Zamowienia_WyrobCukierniczy z_wc in _bakeryContext.zamowienia_WyrobCukiernicze
                    .Where(zwc => zwc.IdZamowienia == z.IdZamowienie).ToList())
                {
                    WyrobDTO productDTO = new WyrobDTO();
                    WyrobCukierniczy product =
                        _bakeryContext.wyrobCukiernicze.FirstOrDefault(w =>
                            w.IdWyrobuCukierniczego == z_wc.IdWyrobuCukierniczego);
                    productDTO.wyrob = product.Nazwa;
                    productsDTO.Add(productDTO);
                }

                orderDTO.wyroby = productsDTO.ToArray();
                ordersDTO.Add(orderDTO);
            }

            return Ok(ordersDTO);
        }


        [HttpPost("{id}")]
        public IActionResult NewOrder(int id, ZamowienieDTO noweZamowienieDTO)
        {
            Zamowienie newOrder = new Zamowienie();
            List<Zamowienia_WyrobCukierniczy> ordersConfectionery = new List<Zamowienia_WyrobCukierniczy>();

            Klient client = _bakeryContext.Klienci.Find(id);
            if (client == null)
                return NotFound("Nie znaleziono klienta o id: " + id);

            Pracownik worker = _bakeryContext.Pracownicy.Find(1);
            if (worker == null)
                return NotFound("Nie znaleziono pracwnika o id: " + 1);


            foreach (WyrobDTO productDTO in noweZamowienieDTO.wyroby)
            {
                WyrobCukierniczy confectionery =
                    _bakeryContext.wyrobCukiernicze.FirstOrDefault(c => c.Nazwa == productDTO.wyrob);

                if (confectionery == null)
                {
                    return NotFound("Nie ma takiego wyrobu: " + productDTO.wyrob);
                }

                Zamowienia_WyrobCukierniczy orderConfectioneries = new Zamowienia_WyrobCukierniczy();
                orderConfectioneries.wyrobCukierniczy = confectionery;
                orderConfectioneries.zamowienie = newOrder;
                orderConfectioneries.Ilosc = Int32.Parse(productDTO.ilosc);
                orderConfectioneries.Uwagi = productDTO.uwagi;
                ordersConfectionery.Add(orderConfectioneries);
            }
            
            newOrder.DataPrzyjecia = DateTime.Parse(noweZamowienieDTO.dataPrzyjecia);
            newOrder.Uwagi = noweZamowienieDTO.uwagi;
            newOrder.klient = client;
            newOrder.pracownik = worker;
            newOrder.zamowienia_WyrobCukiernicze = ordersConfectionery;

            _bakeryContext.Add(newOrder);
            _bakeryContext.SaveChanges();

            return Ok("Stworzono nowe zamowienie");
        }
    }
}