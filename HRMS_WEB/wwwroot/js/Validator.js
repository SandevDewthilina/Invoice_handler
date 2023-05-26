// import {createApp} from '../lib/vue/dist/vue.esm-browser.js';
//
//
// const app = createApp({
//     data() {
//         return {
//             scanned_fields: [],
//             source_fields: [],
//             rectifiedChassisNumber: '',
//             comparisonResources: []
//         }
//     },
//     computed: {
//         getScannedFields() {
//             return this.scanned_fields
//         },
//         getSourceFields() {
//             return this.source_fields
//         },
//         getDataPairs() {
//             const output = []
//             this.scanned_fields.forEach(item =>{
//                 var sourcePair = this.source_fields.find(i => i.Key === item.Key)
//                 if(sourcePair) {
//                     output.push([item, sourcePair])
//                 } else {
//                     output.push([item, {Key: 'N/A', Value: 'N/A'}])
//                 }
//             })
//            
//             console.log(output)
//            
//             return output
//         },
//         getComparisonResources() {
//             return this.comparisonResources
//         }
//     },
//     methods: {
//        
//     },
//     created() {
//         this.scanned_fields = JSON.parse(model.fieldJson)
//         this.rectifiedChassisNumber = rectifiedChassisNumber
//        
//     }
// });
//
// app.mount("#main");