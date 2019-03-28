using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using vladandartem.Models;

namespace vladandartem.ClassHelpers
{
    public class Cart
    {
        List<CartProduct> cartProduct = new List<CartProduct>();

        private readonly string FieldName;

        private ISession session;

        public Cart(ISession session, string FieldName)
        {
            this.session = session;
            this.FieldName = FieldName;
        }

        public void Add(int id, int count = 1)
        {
            if(!cartProduct.Any(product => product.ProductId == id))
            {
                cartProduct.Add(new CartProduct{ ProductId = id, ProductCount = count });
            }
        }
        public List<CartProduct> Decode()
        {
            var SerializedString = session.GetString(FieldName);

            if(SerializedString != null)
                cartProduct = JsonConvert.DeserializeObject<List<CartProduct>>(SerializedString);

            return cartProduct;
        }

        public void Save()
        {
            session.SetString(FieldName, JsonConvert.SerializeObject(cartProduct));
        }

        public void Delete(int id)
        {   
            if(cartProduct.Any(product => product.ProductId == id))
            {
                CartProduct productBuff = cartProduct.Find(product => product.ProductId == id);

                cartProduct.Remove(productBuff);
            }
        }

        public void Edit(int id, int count)
        {
            int index = cartProduct.FindIndex(x => x.ProductId == id);
            var product = cartProduct[index];
            product.ProductCount = count;
            cartProduct[index] = product;
        }

        public int Count()
        {
            return cartProduct.Count();
        }
    }

    public struct CartProduct
    {
        public int ProductId;
        public Product product;
        public int ProductCount;
    }
}