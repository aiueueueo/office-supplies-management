class Item {
  final int itemId;
  final String itemCode;
  final String itemName;
  final String? itemDescription;
  final String unit;
  final int currentStock;
  final int minimumStock;
  final bool isActive;

  Item({
    required this.itemId,
    required this.itemCode,
    required this.itemName,
    this.itemDescription,
    required this.unit,
    required this.currentStock,
    required this.minimumStock,
    required this.isActive,
  });

  factory Item.fromJson(Map<String, dynamic> json) {
    return Item(
      itemId: json['itemId'],
      itemCode: json['itemCode'],
      itemName: json['itemName'],
      itemDescription: json['itemDescription'],
      unit: json['unit'] ?? '個',
      currentStock: json['currentStock'],
      minimumStock: json['minimumStock'],
      isActive: json['isActive'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'itemId': itemId,
      'itemCode': itemCode,
      'itemName': itemName,
      'itemDescription': itemDescription,
      'unit': unit,
      'currentStock': currentStock,
      'minimumStock': minimumStock,
      'isActive': isActive,
    };
  }
}