/*
   Licensed to the Apache Software Foundation (ASF) under one or more
   contributor license agreements.  See the NOTICE file distributed with
   this work for additional information regarding copyright ownership.
   The ASF licenses this file to You under the Apache License, Version 2.0
   (the "License"); you may not use this file except in compliance with
   the License.  You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
var showControllersOnly = false;
var seriesFilter = "";
var filtersOnlySampleSeries = true;

/*
 * Add header in statistics table to group metrics by category
 * format
 *
 */
function summaryTableHeader(header) {
    var newRow = header.insertRow(-1);
    newRow.className = "tablesorter-no-sort";
    var cell = document.createElement('th');
    cell.setAttribute("data-sorter", false);
    cell.colSpan = 1;
    cell.innerHTML = "Requests";
    newRow.appendChild(cell);

    cell = document.createElement('th');
    cell.setAttribute("data-sorter", false);
    cell.colSpan = 3;
    cell.innerHTML = "Executions";
    newRow.appendChild(cell);

    cell = document.createElement('th');
    cell.setAttribute("data-sorter", false);
    cell.colSpan = 7;
    cell.innerHTML = "Response Times (ms)";
    newRow.appendChild(cell);

    cell = document.createElement('th');
    cell.setAttribute("data-sorter", false);
    cell.colSpan = 1;
    cell.innerHTML = "Throughput";
    newRow.appendChild(cell);

    cell = document.createElement('th');
    cell.setAttribute("data-sorter", false);
    cell.colSpan = 2;
    cell.innerHTML = "Network (KB/sec)";
    newRow.appendChild(cell);
}

/*
 * Populates the table identified by id parameter with the specified data and
 * format
 *
 */
function createTable(table, info, formatter, defaultSorts, seriesIndex, headerCreator) {
    var tableRef = table[0];

    // Create header and populate it with data.titles array
    var header = tableRef.createTHead();

    // Call callback is available
    if(headerCreator) {
        headerCreator(header);
    }

    var newRow = header.insertRow(-1);
    for (var index = 0; index < info.titles.length; index++) {
        var cell = document.createElement('th');
        cell.innerHTML = info.titles[index];
        newRow.appendChild(cell);
    }

    var tBody;

    // Create overall body if defined
    if(info.overall){
        tBody = document.createElement('tbody');
        tBody.className = "tablesorter-no-sort";
        tableRef.appendChild(tBody);
        var newRow = tBody.insertRow(-1);
        var data = info.overall.data;
        for(var index=0;index < data.length; index++){
            var cell = newRow.insertCell(-1);
            cell.innerHTML = formatter ? formatter(index, data[index]): data[index];
        }
    }

    // Create regular body
    tBody = document.createElement('tbody');
    tableRef.appendChild(tBody);

    var regexp;
    if(seriesFilter) {
        regexp = new RegExp(seriesFilter, 'i');
    }
    // Populate body with data.items array
    for(var index=0; index < info.items.length; index++){
        var item = info.items[index];
        if((!regexp || filtersOnlySampleSeries && !info.supportsControllersDiscrimination || regexp.test(item.data[seriesIndex]))
                &&
                (!showControllersOnly || !info.supportsControllersDiscrimination || item.isController)){
            if(item.data.length > 0) {
                var newRow = tBody.insertRow(-1);
                for(var col=0; col < item.data.length; col++){
                    var cell = newRow.insertCell(-1);
                    cell.innerHTML = formatter ? formatter(col, item.data[col]) : item.data[col];
                }
            }
        }
    }

    // Add support of columns sort
    table.tablesorter({sortList : defaultSorts});
}

$(document).ready(function() {

    // Customize table sorter default options
    $.extend( $.tablesorter.defaults, {
        theme: 'blue',
        cssInfoBlock: "tablesorter-no-sort",
        widthFixed: true,
        widgets: ['zebra']
    });

    var data = {"OkPercent": 100.0, "KoPercent": 0.0};
    var dataset = [
        {
            "label" : "FAIL",
            "data" : data.KoPercent,
            "color" : "#FF6347"
        },
        {
            "label" : "PASS",
            "data" : data.OkPercent,
            "color" : "#9ACD32"
        }];
    $.plot($("#flot-requests-summary"), dataset, {
        series : {
            pie : {
                show : true,
                radius : 1,
                label : {
                    show : true,
                    radius : 3 / 4,
                    formatter : function(label, series) {
                        return '<div style="font-size:8pt;text-align:center;padding:2px;color:white;">'
                            + label
                            + '<br/>'
                            + Math.round10(series.percent, -2)
                            + '%</div>';
                    },
                    background : {
                        opacity : 0.5,
                        color : '#000'
                    }
                }
            }
        },
        legend : {
            show : true
        }
    });

    // Creates APDEX table
    createTable($("#apdexTable"), {"supportsControllersDiscrimination": true, "overall": {"data": [0.7840909090909091, 500, 1500, "Total"], "isController": false}, "titles": ["Apdex", "T (Toleration threshold)", "F (Frustration threshold)", "Label"], "items": [{"data": [1.0, 500, 1500, "Twitter-0"], "isController": false}, {"data": [1.0, 500, 1500, "Twitter-1"], "isController": false}, {"data": [0.5, 500, 1500, "Twitter"], "isController": false}, {"data": [0.925, 500, 1500, "Twitter-2"], "isController": false}, {"data": [0.35, 500, 1500, "Quora "], "isController": false}, {"data": [1.0, 500, 1500, "Quora -0"], "isController": false}, {"data": [0.58, 500, 1500, "Facebook"], "isController": false}, {"data": [0.985, 500, 1500, "Facebook-1"], "isController": false}, {"data": [0.415, 500, 1500, "Quora -2"], "isController": false}, {"data": [1.0, 500, 1500, "Facebook-0"], "isController": false}, {"data": [0.87, 500, 1500, "Quora -1"], "isController": false}]}, function(index, item){
        switch(index){
            case 0:
                item = item.toFixed(3);
                break;
            case 1:
            case 2:
                item = formatDuration(item);
                break;
        }
        return item;
    }, [[0, 0]], 3);

    // Create statistics table
    createTable($("#statisticsTable"), {"supportsControllersDiscrimination": true, "overall": {"data": ["Total", 1100, 0, 0.0, 517.7745454545451, 9, 3968, 293.0, 1081.8, 1426.0, 3089.51, 62.81048364072403, 4691.732347042197, 11.107819905213269], "isController": false}, "titles": ["Label", "#Samples", "FAIL", "Error %", "Average", "Min", "Max", "Median", "90th pct", "95th pct", "99th pct", "Transactions/s", "Received", "Sent"], "items": [{"data": ["Twitter-0", 100, 0, 0.0, 183.0, 171, 242, 176.0, 202.8, 214.89999999999998, 241.99, 7.4710496824803885, 4.428639802017184, 0.9411771572655958], "isController": false}, {"data": ["Twitter-1", 100, 0, 0.0, 189.69000000000005, 171, 405, 175.5, 236.0, 252.34999999999985, 403.5899999999993, 7.466029565477079, 4.701265492011348, 0.9405447401821712], "isController": false}, {"data": ["Twitter", 100, 0, 0.0, 675.1699999999997, 568, 1062, 600.0, 895.9, 918.75, 1061.86, 7.037297677691766, 468.51283482142856, 2.6321142681210414], "isController": false}, {"data": ["Twitter-2", 100, 0, 0.0, 301.3800000000001, 220, 623, 241.0, 535.0, 568.3499999999999, 622.98, 7.259001161440186, 474.39904827689463, 0.8861085402148664], "isController": false}, {"data": ["Quora ", 100, 0, 0.0, 1622.3, 830, 3968, 1291.5, 3091.8, 3408.999999999998, 3964.199999999998, 6.303977809998109, 984.9187796483957, 2.320898080438757], "isController": false}, {"data": ["Quora -0", 100, 0, 0.0, 21.02, 9, 105, 13.0, 35.70000000000002, 105.0, 105.0, 6.843221788818176, 2.191969479230822, 0.8487198898241293], "isController": false}, {"data": ["Facebook", 100, 0, 0.0, 552.0199999999998, 463, 887, 523.0, 640.6, 691.0, 886.1799999999996, 7.241653993772177, 1361.5627715620249, 1.6689749438771817], "isController": false}, {"data": ["Facebook-1", 100, 0, 0.0, 340.5600000000001, 280, 670, 316.5, 412.9000000000001, 479.39999999999986, 669.3399999999997, 7.36105999263894, 1381.448259799411, 0.8482471475892528], "isController": false}, {"data": ["Quora -2", 100, 0, 0.0, 1092.6400000000003, 548, 2264, 1004.5, 1806.2, 2084.3999999999996, 2263.0499999999997, 6.982265046781176, 1084.3470573025766, 0.8386900397989108], "isController": false}, {"data": ["Facebook-0", 100, 0, 0.0, 210.83, 167, 312, 205.0, 232.9, 238.95, 311.46999999999974, 7.412348973389667, 2.5828851178563488, 0.8541574012304499], "isController": false}, {"data": ["Quora -1", 100, 0, 0.0, 506.9100000000002, 248, 2135, 274.0, 1346.8, 1433.8, 2129.9399999999973, 6.7778229632641995, 4.183055230784872, 0.8406089026704623], "isController": false}]}, function(index, item){
        switch(index){
            // Errors pct
            case 3:
                item = item.toFixed(2) + '%';
                break;
            // Mean
            case 4:
            // Mean
            case 7:
            // Median
            case 8:
            // Percentile 1
            case 9:
            // Percentile 2
            case 10:
            // Percentile 3
            case 11:
            // Throughput
            case 12:
            // Kbytes/s
            case 13:
            // Sent Kbytes/s
                item = item.toFixed(2);
                break;
        }
        return item;
    }, [[0, 0]], 0, summaryTableHeader);

    // Create error table
    createTable($("#errorsTable"), {"supportsControllersDiscrimination": false, "titles": ["Type of error", "Number of errors", "% in errors", "% in all samples"], "items": []}, function(index, item){
        switch(index){
            case 2:
            case 3:
                item = item.toFixed(2) + '%';
                break;
        }
        return item;
    }, [[1, 1]]);

        // Create top5 errors by sampler
    createTable($("#top5ErrorsBySamplerTable"), {"supportsControllersDiscrimination": false, "overall": {"data": ["Total", 1100, 0, null, null, null, null, null, null, null, null, null, null], "isController": false}, "titles": ["Sample", "#Samples", "#Errors", "Error", "#Errors", "Error", "#Errors", "Error", "#Errors", "Error", "#Errors", "Error", "#Errors"], "items": [{"data": [], "isController": false}, {"data": [], "isController": false}, {"data": [], "isController": false}, {"data": [], "isController": false}, {"data": [], "isController": false}, {"data": [], "isController": false}, {"data": [], "isController": false}, {"data": [], "isController": false}, {"data": [], "isController": false}, {"data": [], "isController": false}, {"data": [], "isController": false}]}, function(index, item){
        return item;
    }, [[0, 0]], 0);

});
