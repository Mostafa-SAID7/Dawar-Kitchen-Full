export const MENU_ITEMS = [
  // BREAKFAST SECTION
  {
    name: 'Fava Beans with Yogurt',
    nameAr: 'الفول بالزبادي',
    price: '45 EGP',
    description: 'Creamy fava bean dip topped with velvety yogurt and drizzle of our house ghee oil.',
    descriptionAr: 'ديب الفول الكريمي مع الزبادي الناعم وقطرة من زيت الغريب.',
    category: 'Breakfast',
    dietary: ['Vegetarian', 'Vegan']
  },
  {
    name: 'Syrian Falafel',
    nameAr: 'الفلافل السورية',
    price: '35 EGP',
    description: 'Golden crispy falafel made fresh daily with chickpeas, herbs, and aromatic spices. Served with tahini sauce.',
    descriptionAr: 'فلافل ذهبية مقلية طازة من الحمص والأعشاب والبهارات. مع صلصة الطحينة.',
    category: 'Breakfast',
    dietary: ['Vegetarian', 'Vegan']
  },
  {
    name: 'Egyptian Scrambled Eggs & Cheese',
    nameAr: 'بيض مخلوط مصري مع الجبن',
    price: '55 EGP',
    description: 'Eggs scrambled with sautéed peppers, onions, tomatoes, and local cheese. Served with warm pita.',
    descriptionAr: 'بيض مخلوط مع الفلفل والبصل والطماطم والجبن المحلية مع خبز الفطير.',
    category: 'Breakfast',
    dietary: ['Vegetarian']
  },
  
  // APPETIZERS & MEZZE
  {
    name: 'Hummus',
    nameAr: 'الحمص بالطحينة',
    price: '40 EGP',
    description: 'Creamy chickpea and tahini dip with a hint of garlic, lemon, and olive oil.',
    descriptionAr: 'ديب الحمص الكريمي مع الطحينة وقليل من الثوم والليمون وزيت الزيتون.',
    category: 'Appetizers & Mezze',
    dietary: ['Vegetarian', 'Vegan']
  },
  {
    name: 'Baba Ganoush',
    nameAr: 'بابا غنوج',
    price: '45 EGP',
    description: 'Roasted eggplant, tahini, garlic, and lemon - a smoky, silky masterpiece.',
    descriptionAr: 'الباذنجان المشوي مع الطحينة والثوم والليمون - طبق سوري تقليدي.',
    category: 'Appetizers & Mezze',
    dietary: ['Vegetarian', 'Vegan']
  },
  {
    name: 'Stuffed Grape Leaves',
    nameAr: 'ورق العنب المحشي',
    price: '50 EGP',
    description: 'Tender grape leaves filled with fragrant rice, herbs, and spices. Syrian tradition.',
    descriptionAr: 'ورق عنب طري محشي بالأرز العطري والأعشاب والبهارات.',
    category: 'Appetizers & Mezze',
    dietary: ['Vegetarian', 'Vegan']
  },
  {
    name: 'Tabbouleh Salad',
    nameAr: 'سلطة التبولة',
    price: '55 EGP',
    description: 'Fresh parsley salad with bulgur wheat, tomatoes, onions, lemon juice, and olive oil.',
    descriptionAr: 'سلطة البقدونس مع البرغل والطماطم والبصل وعصير الليمون وزيت الزيتون.',
    category: 'Appetizers & Mezze',
    dietary: ['Vegetarian', 'Vegan']
  },
  {
    name: 'Fried Kibbeh',
    nameAr: 'الكبة المقلية',
    price: '60 EGP',
    description: 'Crispy bulgur and meat shells filled with spiced minced meat. A Syrian favorite.',
    descriptionAr: 'أصداف برغل واللحم المقلية محشوة باللحم المفروم والبهارات.',
    category: 'Appetizers & Mezze',
    dietary: []
  },
  {
    name: 'Sambusak (Mixed Platter)',
    nameAr: 'السمبوسة (مشكلة)',
    price: '65 EGP',
    description: 'Crispy pastries filled with meat, spinach, cheese, and vegetables.',
    descriptionAr: 'معجنات مقلية محشوة باللحم والسبانخ والجبن والخضار.',
    category: 'Appetizers & Mezze',
    dietary: []
  },

  // SYRIAN DISHES
  {
    name: 'Syrian Kabsa - Chicken',
    nameAr: 'الكبسة السورية - دجاج',
    price: '95 EGP',
    description: 'Fragrant basmati rice with tender chicken, warm spices, and topped with crispy fried onions.',
    descriptionAr: 'أرز باسماتي عطري مع الدجاج الطري والبهارات الدافئة والبصل المقلي.',
    category: 'Syrian Dishes',
    dietary: []
  },
  {
    name: 'Syrian Kabsa - Meat',
    nameAr: 'الكبسة السورية - لحم',
    price: '105 EGP',
    description: 'Premium beef or lamb with fragrant rice and warm spices.',
    descriptionAr: 'لحم بقري أو ضأن مع الأرز العطري والبهارات الدافئة.',
    category: 'Syrian Dishes',
    dietary: []
  },
  {
    name: 'Syrian Kabsa - Vegetarian',
    nameAr: 'الكبسة السورية - نباتي',
    price: '75 EGP',
    description: 'Aromatic rice with mixed vegetables, chickpeas, and warm spices.',
    descriptionAr: 'أرز عطري مع الخضار المختلطة والحمص والبهارات الدافئة.',
    category: 'Syrian Dishes',
    dietary: ['Vegetarian', 'Vegan']
  },
  {
    name: 'Maalouba - Chicken',
    nameAr: 'المعكوسة - دجاج',
    price: '85 EGP',
    description: 'Rice layered with chicken and eggplant, then flipped upside down. A showstopper!',
    descriptionAr: 'أرز مع الدجاج والباذنجان، مقلوبة بطريقة تقليدية.',
    category: 'Syrian Dishes',
    dietary: []
  },
  {
    name: 'Chicken Shawerma Fatta',
    nameAr: 'فتة الشاورما المصرية',
    price: '80 EGP',
    description: 'Tender shawarma chicken over crispy pita bread, drizzled with yogurt and tahini.',
    descriptionAr: 'شاورما الدجاج على خبز مقرمش مع الزبادي والطحينة.',
    category: 'Syrian Dishes',
    dietary: []
  },
  {
    name: 'Kibbeh Labaneye',
    nameAr: 'الكبة بالنوى',
    price: '75 EGP',
    description: 'Bulgur and meat shells in a creamy yogurt sauce with pine nuts.',
    descriptionAr: 'أصداف البرغل واللحم مع صلصة الزبادي الكريمية وجوز الصنوبر.',
    category: 'Syrian Dishes',
    dietary: []
  },

  // EGYPTIAN DISHES
  {
    name: 'Koshari',
    nameAr: 'الكشري المصري',
    price: '45 EGP',
    description: 'Egypt\'s beloved national dish — layers of rice, lentils, and pasta with crispy onions and spiced tomato sauce.',
    descriptionAr: 'الطبق المصري الوطني - الأرز والعدس والمكرونة مع البصل المقلي والطماطم.',
    category: 'Egyptian Dishes',
    dietary: ['Vegetarian', 'Vegan']
  },
  {
    name: 'Egyptian Fatta',
    nameAr: 'الفتة المصرية',
    price: '70 EGP',
    description: 'Crispy pita bread with rice, lentils, chickpeas, topped with yogurt, garlic, and vinegar.',
    descriptionAr: 'خبز مقرمش مع الأرز والعدس والحمص والزبادي والثوم والخل.',
    category: 'Egyptian Dishes',
    dietary: ['Vegetarian']
  },
  {
    name: 'Hawawshi',
    nameAr: 'الحاويشي',
    price: '55 EGP',
    description: 'Egyptian baked pastry stuffed with spiced minced meat, peppers, and onions.',
    descriptionAr: 'معجنات مصرية محشوة باللحم المفروم والفلفل والبصل.',
    category: 'Egyptian Dishes',
    dietary: []
  },
  {
    name: 'Mombar',
    nameAr: 'الممبار',
    price: '65 EGP',
    description: 'Intestines stuffed with rice, herbs, and spices - a traditional Egyptian delicacy.',
    descriptionAr: 'أمعاء محشوة بالأرز والأعشاب والبهارات - لذة مصرية تقليدية.',
    category: 'Egyptian Dishes',
    dietary: []
  },
  {
    name: 'Rokak with Minced Meat',
    nameAr: 'الرقاق باللحم المفروم',
    price: '70 EGP',
    description: 'Thin crispy pastry layers with spiced minced meat, baked until golden.',
    descriptionAr: 'طبقات رقاق رقيقة مع اللحم المفروم والبهارات.',
    category: 'Egyptian Dishes',
    dietary: []
  },
  {
    name: 'Stuffed Mahshi - Mixed',
    nameAr: 'المحشي - مشكلة',
    price: '75 EGP',
    description: 'Zucchini, tomatoes, and peppers stuffed with rice and spiced meat.',
    descriptionAr: 'كوسة وطماطم وفلفل محشي بالأرز واللحم المفروم.',
    category: 'Egyptian Dishes',
    dietary: []
  },
  {
    name: 'Molokhia with Meat',
    nameAr: 'الملوخية باللحم',
    price: '80 EGP',
    description: 'Velvety green molokhia stew with tender meat and garlic coriander crisps.',
    descriptionAr: 'الملوخية الناعمة مع اللحم الطري وقطع الثوم والكزبرة.',
    category: 'Egyptian Dishes',
    dietary: []
  },

  // GRILLS & MEAT
  {
    name: 'Shish Tawook',
    nameAr: 'شيش الطاووق',
    price: '90 EGP',
    description: 'Marinated chicken breast cubes grilled on skewers with peppers and onions.',
    descriptionAr: 'صدور الدجاج المتبلة مشوية على أسياخ مع الفلفل والبصل.',
    category: 'Grills & Meat',
    dietary: []
  },
  {
    name: 'Charcoal Kofta Platter',
    nameAr: 'طبق الكفتة بالفحم',
    price: '100 EGP',
    description: 'Hand-seasoned minced beef and lamb kofta grilled over charcoal, with warm bread and tahini.',
    descriptionAr: 'كفتة لحم بقري وضأن مطهوة بالفحم مع الخبز والطحينة.',
    category: 'Grills & Meat',
    dietary: []
  },
  {
    name: 'Grilled Chicken Breast',
    nameAr: 'صدور الدجاج المشوية',
    price: '85 EGP',
    description: 'Tender, marinated chicken breast grilled to perfection.',
    descriptionAr: 'صدور دجاج طرية ومتبلة مشوية بشكل مثالي.',
    category: 'Grills & Meat',
    dietary: []
  },
  {
    name: 'Grilled Lamb Chops',
    nameAr: 'شرائح الضأن المشوية',
    price: '120 EGP',
    description: 'Premium lamb chops marinated with herbs and spices, grilled on charcoal.',
    descriptionAr: 'شرائح ضأن فاخرة متبلة بالأعشاب والبهارات، مشوية بالفحم.',
    category: 'Grills & Meat',
    dietary: []
  },
  {
    name: 'Metafaya (Egyptian Meatballs)',
    nameAr: 'المتافايا',
    price: '75 EGP',
    description: 'Spiced meatballs with herbs, grilled until golden and juicy.',
    descriptionAr: 'كرات لحم متبلة بالأعشاب والبهارات.',
    category: 'Grills & Meat',
    dietary: []
  },

  // SIDES & RICE
  {
    name: 'Fragrant White Rice',
    nameAr: 'الأرز الأبيض العطري',
    price: '25 EGP',
    description: 'Perfectly cooked basmati rice with warm spices.',
    descriptionAr: 'أرز باسماتي مطهو بشكل مثالي مع البهارات الدافئة.',
    category: 'Sides & Rice',
    dietary: ['Vegetarian', 'Vegan']
  },
  {
    name: 'Rice with Vermicelli',
    nameAr: 'الأرز بالشعيرية',
    price: '30 EGP',
    description: 'Rice cooked with toasted vermicelli for extra texture and flavor.',
    descriptionAr: 'أرز مع الشعيرية المحمصة.',
    category: 'Sides & Rice',
    dietary: ['Vegetarian', 'Vegan']
  },
  {
    name: 'Hummus (Side)',
    nameAr: 'حمص (جانب)',
    price: '30 EGP',
    description: 'Creamy chickpea dip - perfect side for any grilled dish.',
    descriptionAr: 'ديب الحمص الكريمي - طبق جانبي مثالي.',
    category: 'Sides & Rice',
    dietary: ['Vegetarian', 'Vegan']
  },

  // DESSERTS
  {
    name: 'Om Ali',
    nameAr: 'أم علي',
    price: '40 EGP',
    description: 'Egypt\'s legendary warm dessert — flaky pastry in sweet cream with almonds, sultanas, and coconut.',
    descriptionAr: 'الحلوى المصرية التقليدية - معجنات رقيقة مع الكريمة الحلوة واللوز والزبيب وجوز الهند.',
    category: 'Desserts',
    dietary: ['Vegetarian']
  },
  {
    name: 'Kunafa (Cheese)',
    nameAr: 'الكنافة (جبن)',
    price: '45 EGP',
    description: 'Crispy shredded pastry with melted cheese and honey syrup.',
    descriptionAr: 'معجنات مقرمشة مع الجبن المذاب وشراب العسل.',
    category: 'Desserts',
    dietary: ['Vegetarian']
  },
  {
    name: 'Baklava',
    nameAr: 'البقلاوة',
    price: '50 EGP',
    description: 'Layers of phyllo pastry with pistachios and walnuts, drizzled with honey.',
    descriptionAr: 'طبقات من العجين مع الفستق والجوز وشراب العسل.',
    category: 'Desserts',
    dietary: ['Vegetarian']
  },

  // BEVERAGES
  {
    name: 'Karkade (Hibiscus Tea)',
    nameAr: 'الكركديه',
    price: '15 EGP',
    description: 'Traditional Egyptian hibiscus tea, served hot or cold.',
    descriptionAr: 'شاي الكركديه المصري التقليدي، ساخن أو بارد.',
    category: 'Beverages',
    dietary: ['Vegetarian', 'Vegan']
  },
  {
    name: 'Fresh Mint Tea',
    nameAr: 'شاي النعناع الطازج',
    price: '12 EGP',
    description: 'Fresh mint steeped in hot water with a touch of honey.',
    descriptionAr: 'النعناع الطازج مع قليل من العسل.',
    category: 'Beverages',
    dietary: ['Vegetarian', 'Vegan']
  },
  {
    name: 'Fresh Lemonade',
    nameAr: 'عصير الليمون الطازج',
    price: '18 EGP',
    description: 'Freshly squeezed lemon juice with a touch of sugar.',
    descriptionAr: 'عصير الليمون الطازج مع السكر.',
    category: 'Beverages',
    dietary: ['Vegetarian', 'Vegan']
  }
];
