using DepoX.Features.Split;
using DepoX.Services.Erp.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DepoX.Features.Split;

public static class SplitMapper
{
    // ======================================================
    // ERP → DOMAIN MODEL
    // ======================================================
    public static SplitBarcodeModel ToModel(
        ErpBarcodeDetailDto dto)
    {
        return new SplitBarcodeModel
        {
            Barcode = dto.Barcode,
            ItemCode = dto.ItemCode,
            ItemName = dto.ItemName,
            LotCode = dto.LotCode,
            ColorCode = dto.ColorCode,
            UnitCode = dto.UnitCode,
            Quantity = dto.Quantity,
            ExistingSplits = dto.ExistingSplits
                .Select(ToModel)
                .ToList()
        };
    }

    private static SplitBarcodeModel ToModel(
        ErpSplitBarcodeDto dto)
    {
        return new SplitBarcodeModel
        {
            Barcode = dto.Barcode,
            ItemCode = dto.ItemCode,
            ItemName = dto.ItemName,
            LotCode = dto.LotCode,
            ColorCode = dto.ColorCode,
            UnitCode = dto.UnitCode,
            Quantity = dto.Quantity
        };
    }

    // ======================================================
    // DOMAIN MODEL → VIEWMODEL
    // ======================================================
    public static SplitRowVm ToVm(
        SplitBarcodeModel model,
        bool isExisting)
    {
        return new SplitRowVm
        {
            IsExisting = isExisting,
            ItemCode = model.ItemCode,
            ItemName = model.ItemName,
            LotCode = model.LotCode,
            ColorCode = model.ColorCode,
            UnitCode = model.UnitCode,
            Quantity = model.Quantity
        };
    }

    // ======================================================
    // VIEWMODEL → DOMAIN MODEL (SAVE)
    // ======================================================
    public static SplitBarcodeModel ToModel(
        SplitRowVm vm)
    {
        return new SplitBarcodeModel
        {
            ItemCode = vm.ItemCode,
            ItemName = vm.ItemName,
            LotCode = vm.LotCode,
            ColorCode = vm.ColorCode,
            UnitCode = vm.UnitCode,
            Quantity = vm.Quantity
        };
    }

    //// ======================================================
    //// DOMAIN MODEL → ERP SAVE DTO
    //// ======================================================
    //public static ErpSplitSaveDto ToErpSaveDto(
    //    SplitBarcodeModel model)
    //{
    //    return new ErpSplitSaveDto
    //    {
    //        ItemCode = model.ItemCode,
    //        ItemName = model.ItemName,
    //        LotCode = model.LotCode,
    //        ColorCode = model.ColorCode,
    //        UnitCode = model.UnitCode,
    //        Quantity = model.Quantity
    //    };
    //}
}
