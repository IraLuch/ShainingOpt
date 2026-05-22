
        $(document).ready(function () {
        //Функция форматирования выпадающего списка
        function formatProduct(state) {
        if (!state.id) return state.text;

        var img = $(state.element).data('img');
        var price = $(state.element).data('price');
        var size = $(state.element).data('size');

        var $state = $(
        '<div class="d-flex align-items-center">' +
        '<img src="' + img + '" style="width: 40px; height: 40px; object-fit: cover; margin-right: 10px; border-radius: 4px;" />' +
        '<div>' +
        '<div style="font-weight: bold;">' + state.text + '</div>' +
        '<div style="font-size: 0.85em; color: #666;">Цена: ' + price + ' ₽ | Размер: ' + size + '</div>' +
        '</div>' +
        '</div>'
        );
        return $state;
        };

        $('#productSelect').select2({
        templateResult: formatProduct,
        templateSelection: formatProduct,
        width: '100%'
        });

        //Обработка изменения выбора товара
        $('#productSelect').on('change', function () {
        var selectedOption = $(this).find(':selected');
        var val = $(this).val();

        $('input[name="variantId"]').val(val);

        var min = selectedOption.data('min');
        var max = selectedOption.data('max');
        var $qtyInput = $('#productQty');

        if (min !== undefined && max !== undefined) {
        //Устанавливаем лимиты и начальное значение
        $qtyInput.attr('min', min);
        $qtyInput.attr('max', max);
        $qtyInput.val(min);

        if (max <= 0) {
        $qtyInput.val(0).prop('disabled', true);
        $('#btnAddProduct').prop('disabled', true).text('Нет в наличии');
        } else {
        $qtyInput.prop('disabled', false);
        $('#btnAddProduct').prop('disabled', false).text('Добавить');
        }
        }
        });

        if ($.validator) {
        $.validator.methods.range = function (value, element, param) {
        var globalizedValue = value.replace(",", ".");
        return this.optional(element) || (globalizedValue >= param[0] && globalizedValue <= param[1]);
        };

        $.validator.methods.number = function (value, element) {
        return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:[\s\.,]\d{3})+)(?:[\.,]\d+)?$/.test(value);
        };
        }
        }); 