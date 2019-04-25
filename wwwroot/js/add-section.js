new Vue ({
    el: '#sections',
    data: {
        sections: [],
        modalId: 'id',
        sectionId: '',
        sectionName: '',
        index: '',
        addSectionName: ''
    },
    mounted: function () {
        axios
            .post('/AdminMenu/GetSections')
            .then(response => {
                this.sections = response.data;
            });
    },
    methods: {
        activateModal: function (sectionId, sectionName, index) {
            this.modalId = 'id';
            this.modalId += sectionId;
            this.sectionName = sectionName;
            this.sectionId = sectionId;
            this.index = index
        },
        deleteSection: function (event) {
            const data = new FormData(document.querySelector('.section-delete'));
            data.append('id', this.sectionId);
            axios
            .post('/AdminMenu/SectionDelete', data 
            )
            .then(
                this.sections.splice(this.index, 1)
            );
            event.preventDefault();
        },
        closeModal: function () {
            $('.close-modal').trigger('click');
        },
        addSection: function (event) {
            const data = new FormData(document.querySelector('.add-new-category'));
            data.append('sectionName', this.addSectionName);
            axios
            .post('/AdminMenu/SectionAdd', data
            )
            .then(response => {
                this.sections.push(response.data);
                this.addSectionName = ''
            });
            event.preventDefault();
        }
    }
})
