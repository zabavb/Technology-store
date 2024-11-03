﻿using Client.Models.Products;
using Client.Models.Users;
using Library.Models;

namespace Client.Models
{
    public static class ModelExtension
    {
        public static User ToUser(ManageUserViewModel model) =>
            new User(
                model.Id, model.Username, model.FirstName,
                model.LastName, model.Age, model.Email, model.Phone,
                model.Password, model.Role, model.Basket, model.Purchases
            );

        public static Product ToProduct(ManageProductViewModel model) =>
            new Product(model.Id, model.Brand, model.Model, model.Producer, model.Price, model.Details);
    }
}