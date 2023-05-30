import {createApp} from '../lib/vue/dist/vue.esm-browser.js';


const app = createApp({
    data() {
        return {
            pairsCollection: []
        }
    },
    computed: {
        getPairsCollection() {
            return this.pairsCollection
        }
    },
    methods: {
        updateValue(sourceId, key, collectionPlace) {
            
            axios.post('/api/ExternalDataApi/EditExternalData', {
                key: key,
                sourceId: sourceId,
                newValue: this.pairsCollection.find(c => c[collectionPlace].Key === key)[collectionPlace].Value
            }).then(resp => {
                if(resp.data.success) {
                    location.reload()
                } else {
                    alert('request failed')
                }
            }).catch(err => {
                alert(err.message)
            })
        }
    },
    created() {
        this.pairsCollection = pairsCollection
    }
});

app.mount("#main");