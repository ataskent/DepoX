using DepoX.Features.Basket;
using DepoX.Services.Erp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepoX.Features.FromWorder
{
    public static class FromWorderMapper
    {
        public static TransferMVm ToModel(this TransferM dto)
        {
            return new TransferMVm
            {
                Code = dto.Code,
                Name = dto.Code,
                WhouseCode = dto.WhouseCode
            };
        }
        public static WhouseVm ToModel(this Whouse dto)
        {
            return new WhouseVm
            {
                Code = dto.Code,
                Name = dto.Name
            };
        }
        public static BarcodeVm ToModel(this Barcode dto)
        {
            return new BarcodeVm
            {
                Code = dto.Code,
                Quantity = dto.Quantity,
                WhouseCode = dto.WhouseCode,
                LotCode = dto.LotCode,
                ItemCode = dto.ItemCode,
                ItemName = dto.ItemName,
                UnitCode = dto.UnitCode,
                ColorCode = dto.ColorCode

            };
        }
        public static ItemsVm ToModel(this Items dto)
        {
            return new ItemsVm
            {
                Code = dto.Code,
                Quantity = dto.Quantity,
                WhouseCode = dto.WhouseCode,
                Name = dto.Name,
                UnitCode = dto.UnitCode
            };
        }

        public static TransferData ToModel(this TransferDataVm vm)
        {
            return new TransferData
            {
                TransferM = new TransferM
                {
                    Code = vm.Transfer.Code,
                    Name = vm.Transfer.Name,
                    WhouseCode = vm.Transfer.WhouseCode
                },
                Barcodes = vm.Barcodes.Select(b => new Barcode
                {
                    Code = b.Code,
                    Quantity = b.Quantity,
                    WhouseCode = b.WhouseCode,
                    LotCode = b.LotCode,
                    ItemCode = b.ItemCode,
                    ItemName = b.ItemName,
                    UnitCode = b.UnitCode,
                    ColorCode = b.ColorCode

                }).ToList(),
                Items = vm.Items.Select(i => new Items
                {
                    Code = i.Code,
                    Quantity = i.Quantity,
                    WhouseCode = i.WhouseCode,
                    Name = i.Name,
                    UnitCode = i.UnitCode

                }).ToList()
            };
        }
    }
}
