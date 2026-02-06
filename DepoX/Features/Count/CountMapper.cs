using DepoX.Services.Erp.Dtos;

namespace DepoX.Features.Count;

public static class CountMapper
{
    public static CountMVm ToModel(this CountM vm)
    {
        return new CountMVm
        {
            DocNo = vm.DocNo,
            CreatedAt = vm.CreatedAt,
            WhouseCode = vm.WhouseCode,
            WhouseName = vm.WhouseName,
            Items = vm.Items.Select(x => new ItemVm
            {
                Code = x.Code,
                Name = x.Name
            }).ToList(),
            Barcodes = vm.Barcodes.Select(x => new BarcodeVm
            {
                Code = x.Code,
                Quantity = x.Quantity
            }).ToList()
        };
    }

    public static CountListVm ToModel(this CountList vm)
    {
        return new CountListVm
        {
            Counts = vm.Counts.Select(x => new CountVm
            {
                Code = x.Code,
                Name = x.Name
            }).ToList(),
            Whouses = vm.Whouses.Select(x => new WhouseVm
            {
                Code = x.Code,
                Name = x.Name
            }).ToList()
        };
    }

    public static CountVm ToModel(this DepoX.Services.Erp.Dtos.Count vm)
    {
        return new CountVm
        {
            Code = vm.Code,
            Name = vm.Code
        };
    }

    public static WhouseVm ToModel(this DepoX.Services.Erp.Dtos.WhouseCount vm)
    {
        return new WhouseVm
        {
            Code = vm.Code,
            Name = vm.Name
        };
    }

    public static ItemVm ToModel(this DepoX.Services.Erp.Dtos.ItemCount vm)
    {
        return new ItemVm
        {
            Quantity = vm.Quantity,
            Code = vm.Code,
            Name = vm.Name
        };
    }

    public static BarcodeVm ToModel(this DepoX.Services.Erp.Dtos.BarcodeCount vm)
    {
        return new BarcodeVm
        {
            Code = vm.Code,
            Quantity = vm.Quantity,
            IsEditing = vm.IsEditing,
            IsSelected = vm.IsSelected
        };
    }

    public static CountM ToModel(this CountMVm vm)
    {
        return new CountM
        {
            DocNo = vm.DocNo,
            CreatedAt = vm.CreatedAt,
            WhouseCode = vm.WhouseCode,
            WhouseName = vm.WhouseName,
            Items = vm.Items.Select(x => new ItemCount
            {
                Quantity = x.Quantity,
                Code = x.Code,
                Name = x.Name
            }).ToList(),
            Barcodes = vm.Barcodes.Select(x => new BarcodeCount
            {
                Code = x.Code,
                Quantity = x.Quantity
            }).ToList()
        };
    }

    public static CountList ToModel(this CountListVm vm)
    {
        return new CountList
        {
            Counts = vm.Counts.Select(x => new DepoX.Services.Erp.Dtos.Count
            {
                Code = x.Code,
                Name = x.Name
            }).ToList(),
            Whouses = vm.Whouses.Select(x => new WhouseCount
            {
                Code = x.Code,
                Name = x.Name
            }).ToList()
        };
    }

    public static DepoX.Services.Erp.Dtos.Count ToModel(this CountVm vm)
    {
        return new DepoX.Services.Erp.Dtos.Count
        {
            Code = vm.Code,
            Name = vm.Code
        };
    }

    public static DepoX.Services.Erp.Dtos.WhouseCount ToModel(this WhouseVm vm)
    {
        return new DepoX.Services.Erp.Dtos.WhouseCount
        {
            Code = vm.Code,
            Name = vm.Name
        };
    }

    public static DepoX.Services.Erp.Dtos.ItemCount ToModel(this ItemVm vm)
    {
        return new DepoX.Services.Erp.Dtos.ItemCount
        {
            Quantity = vm.Quantity,
            Code = vm.Code,
            Name = vm.Name
        };
    }

    public static DepoX.Services.Erp.Dtos.BarcodeCount ToModel(this BarcodeVm vm)
    {
        return new DepoX.Services.Erp.Dtos.BarcodeCount
        {
            Code = vm.Code,
            Quantity = vm.Quantity,
            IsEditing = vm.IsEditing,
            IsSelected = vm.IsSelected
        };
    }


}


