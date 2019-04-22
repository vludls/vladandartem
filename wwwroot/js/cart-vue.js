new Vue ({
    el: '#cart',
    data: {
        test: 'hello',
        quantity: 1,
        price: 0,
        priceOfUnit: 0,
        products: [],
        ImgPath: ''
    },
    mounted: function () {
        axios
          .post('/PersonalArea/CartGetCartProducts')
          .then(response => {
            this.products = response.data;
            this.ImgPath = response.data[0].Product.ImgPath
            //this.ImgPath = response.data
          });
        },
    watch: {
        quantity: function() {
            this.price = this.quantity * this.$refs.priceOfUnit.value
        }
    }
    
})