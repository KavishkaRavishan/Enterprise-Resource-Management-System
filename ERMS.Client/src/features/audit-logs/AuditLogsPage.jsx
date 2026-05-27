import { useState, useEffect } from 'react';
import api from '../../api/axios';
import { 
  ClipboardList, 
  Search, 
  User, 
  Calendar, 
  Activity, 
  Filter, 
  ArrowRight, 
  ChevronDown, 
  ChevronUp,
  Database
} from 'lucide-react';

export default function AuditLogsPage() {
  const [logs, setLogs] = useState([]);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');
  const [actionFilter, setActionFilter] = useState('All');
  const [entityFilter, setEntityFilter] = useState('All');
  const [expandedRow, setExpandedRow] = useState(null);

  const fetchLogs = () => {
    setLoading(true);
    api.get('/auditlogs')
      .then((r) => setLogs(r.data.data || []))
      .catch(() => {})
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    fetchLogs();
  }, []);

  const toggleRow = (id) => {
    setExpandedRow(expandedRow === id ? null : id);
  };

  // Extract unique entity names for filters
  const entities = ['All', ...new Set(logs.map(log => log.entityName))];

  // Filtering
  const filteredLogs = logs.filter((log) => {
    const matchesSearch = 
      `${log.entityName} ${log.action} ${log.userName || ''}`
        .toLowerCase()
        .includes(search.toLowerCase());
        
    const matchesAction = actionFilter === 'All' || log.action === actionFilter;
    const matchesEntity = entityFilter === 'All' || log.entityName === entityFilter;

    return matchesSearch && matchesAction && matchesEntity;
  });

  return (
    <div className="space-y-6 animate-fade-in">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-surface-900 flex items-center gap-2">
            <ClipboardList className="w-7 h-7 text-primary-600" />
            System Audit Trail
          </h1>
          <p className="text-surface-500 mt-1">
            Real-time tracking of data updates and administrator interactions
          </p>
        </div>
      </div>

      {/* Filters Toolbar */}
      <div className="bg-white border border-surface-200 rounded-xl p-4 flex flex-col md:flex-row md:items-center gap-4">
        {/* Search */}
        <div className="relative flex-1">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-4 h-4 text-surface-400" />
          <input
            type="text"
            placeholder="Search audit trail by entity, user..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            className="w-full pl-10 pr-4 py-2 border border-surface-200 rounded-lg focus:ring-2 focus:ring-primary-500 outline-none text-surface-900 text-sm bg-white"
          />
        </div>

        {/* Action filter */}
        <div className="flex items-center gap-2">
          <Filter className="w-4 h-4 text-surface-400" />
          <select
            value={actionFilter}
            onChange={(e) => setActionFilter(e.target.value)}
            className="border border-surface-200 rounded-lg py-2 px-3 text-sm focus:ring-2 focus:ring-primary-500 outline-none bg-white text-surface-700"
          >
            <option value="All">All Actions</option>
            <option value="Added">Added</option>
            <option value="Modified">Modified</option>
            <option value="Deleted">Deleted</option>
          </select>
        </div>

        {/* Entity filter */}
        <div className="flex items-center gap-2">
          <Database className="w-4 h-4 text-surface-400" />
          <select
            value={entityFilter}
            onChange={(e) => setEntityFilter(e.target.value)}
            className="border border-surface-200 rounded-lg py-2 px-3 text-sm focus:ring-2 focus:ring-primary-500 outline-none bg-white text-surface-700"
          >
            <option value="All">All Entities</option>
            {entities.filter(e => e !== 'All').map((entity) => (
              <option key={entity} value={entity}>
                {entity}
              </option>
            ))}
          </select>
        </div>
      </div>

      {/* Logs Table */}
      {loading ? (
        <div className="space-y-3">
          {[...Array(6)].map((_, i) => (
            <div key={i} className="h-16 bg-surface-200 rounded-xl animate-pulse" />
          ))}
        </div>
      ) : (
        <div className="bg-white border border-surface-200 rounded-xl overflow-hidden shadow-sm">
          <div className="overflow-x-auto">
            <table className="w-full border-collapse">
              <thead>
                <tr className="bg-surface-50 border-b border-surface-200">
                  <th className="w-10"></th>
                  <th className="text-left px-5 py-3 text-xs font-semibold text-surface-500 uppercase tracking-wider">
                    Timestamp
                  </th>
                  <th className="text-left px-5 py-3 text-xs font-semibold text-surface-500 uppercase tracking-wider">
                    Entity
                  </th>
                  <th className="text-left px-5 py-3 text-xs font-semibold text-surface-500 uppercase tracking-wider">
                    Action
                  </th>
                  <th className="text-left px-5 py-3 text-xs font-semibold text-surface-500 uppercase tracking-wider">
                    Performed By
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-surface-100">
                {filteredLogs.map((log) => {
                  const isExpanded = expandedRow === log.id;
                  return (
                    <>
                      <tr 
                        key={log.id} 
                        className={`hover:bg-surface-50/70 transition-colors cursor-pointer ${
                          isExpanded ? 'bg-surface-50/40' : ''
                        }`}
                        onClick={() => toggleRow(log.id)}
                      >
                        <td className="pl-4 py-4 text-center">
                          {isExpanded ? (
                            <ChevronUp className="w-4 h-4 text-surface-400" />
                          ) : (
                            <ChevronDown className="w-4 h-4 text-surface-400" />
                          )}
                        </td>
                        <td className="px-5 py-4 text-sm text-surface-600 whitespace-nowrap">
                          <span className="flex items-center gap-2">
                            <Calendar className="w-4 h-4 text-surface-400" />
                            {new Date(log.timestamp).toLocaleString()}
                          </span>
                        </td>
                        <td className="px-5 py-4 text-sm font-semibold text-surface-800">
                          {log.entityName}
                          <span className="block text-xs font-normal text-surface-400 font-mono mt-0.5">
                            ID: {log.entityId}
                          </span>
                        </td>
                        <td className="px-5 py-4 whitespace-nowrap">
                          <span
                            className={`text-xs px-2.5 py-1 rounded-full font-medium inline-flex items-center gap-1 ${
                              log.action === 'Added'
                                ? 'bg-emerald-50 text-emerald-700 border border-emerald-100'
                                : log.action === 'Modified'
                                ? 'bg-blue-50 text-blue-700 border border-blue-100'
                                : 'bg-red-50 text-red-700 border border-red-100'
                            }`}
                          >
                            <Activity className="w-3 h-3" />
                            {log.action}
                          </span>
                        </td>
                        <td className="px-5 py-4 text-sm text-surface-600 whitespace-nowrap">
                          <div className="flex items-center gap-2">
                            <div className="w-6 h-6 rounded-full bg-surface-200 text-surface-700 flex items-center justify-center text-xs font-semibold">
                              <User className="w-3 h-3" />
                            </div>
                            <span className="font-medium text-surface-700">
                              {log.userName || 'System'}
                            </span>
                          </div>
                        </td>
                      </tr>

                      {/* Expanded Details Pane */}
                      {isExpanded && (
                        <tr className="bg-surface-50/20">
                          <td colSpan={5} className="px-8 py-4 border-t border-surface-100">
                            <div className="bg-white border border-surface-200 rounded-xl p-5 space-y-4 shadow-inner max-w-4xl">
                              <h4 className="text-sm font-semibold text-surface-800 border-b border-surface-100 pb-2">
                                Change Audit Details
                              </h4>
                              
                              <DiffViewer 
                                action={log.action}
                                oldValues={log.oldValues} 
                                newValues={log.newValues} 
                              />
                            </div>
                          </td>
                        </tr>
                      )}
                    </>
                  );
                })}
                {filteredLogs.length === 0 && (
                  <tr>
                    <td colSpan={5} className="text-center py-12 text-surface-400">
                      <ClipboardList className="w-12 h-12 mx-auto mb-3 text-surface-300" />
                      <p className="text-base font-medium text-surface-500">No audit logs found</p>
                      <p className="text-sm text-surface-400 mt-1">Try adjusting your filters or search terms</p>
                    </td>
                  </tr>
                )}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
}

function DiffViewer({ action, oldValues, newValues }) {
  try {
    const oldObj = oldValues ? JSON.parse(oldValues) : null;
    const newObj = newValues ? JSON.parse(newValues) : null;

    // Keys that exist in either object
    const allKeys = Array.from(
      new Set([
        ...Object.keys(oldObj || {}),
        ...Object.keys(newObj || {})
      ])
    ).filter(key => key.toLowerCase() !== 'id' && key.toLowerCase() !== 'updated' && key.toLowerCase() !== 'created');

    if (allKeys.length === 0) {
      return <p className="text-sm text-surface-500 italic">No direct property updates recorded.</p>;
    }

    return (
      <div className="space-y-2">
        <div className="grid grid-cols-3 gap-4 text-xs font-semibold text-surface-400 uppercase tracking-wider pb-1 border-b border-surface-100">
          <div>Property</div>
          <div>Original State</div>
          <div>New State</div>
        </div>
        <div className="divide-y divide-surface-100 max-h-80 overflow-y-auto pr-2">
          {allKeys.map((key) => {
            const oldVal = oldObj?.[key];
            const newVal = newObj?.[key];

            const formatValue = (val) => {
              if (val === null || val === undefined) return <span className="text-surface-400 italic">null</span>;
              if (typeof val === 'boolean') return val ? 'True' : 'False';
              if (typeof val === 'object') return JSON.stringify(val);
              return val.toString();
            };

            return (
              <div key={key} className="grid grid-cols-3 gap-4 py-2.5 text-sm items-center hover:bg-surface-50/50 rounded px-1">
                <div className="font-semibold text-surface-700 font-mono text-xs">{key}</div>
                
                {/* Old Value Column */}
                <div className="text-surface-600 truncate max-w-xs font-mono text-xs">
                  {action === 'Added' ? (
                    <span className="text-surface-300 italic">—</span>
                  ) : (
                    <span className="line-through bg-red-50 text-red-600 px-1.5 py-0.5 rounded border border-red-100">
                      {formatValue(oldVal)}
                    </span>
                  )}
                </div>

                {/* New Value Column */}
                <div className="text-surface-600 truncate max-w-xs font-mono text-xs flex items-center gap-1.5">
                  {action !== 'Added' && action !== 'Deleted' && (
                    <ArrowRight className="w-3.5 h-3.5 text-surface-400 flex-shrink-0" />
                  )}
                  {action === 'Deleted' ? (
                    <span className="text-surface-300 italic">—</span>
                  ) : (
                    <span className="bg-emerald-50 text-emerald-700 font-semibold px-1.5 py-0.5 rounded border border-emerald-100">
                      {formatValue(newVal)}
                    </span>
                  )}
                </div>
              </div>
            );
          })}
        </div>
      </div>
    );
  } catch (e) {
    return (
      <div className="grid grid-cols-2 gap-4">
        <div>
          <span className="text-xs font-bold text-surface-400 uppercase">Old Data Payload</span>
          <pre className="mt-1 p-3 bg-surface-100 border border-surface-200 rounded-lg text-xs font-mono text-surface-700 overflow-x-auto">
            {oldValues || 'None'}
          </pre>
        </div>
        <div>
          <span className="text-xs font-bold text-surface-400 uppercase">New Data Payload</span>
          <pre className="mt-1 p-3 bg-surface-100 border border-surface-200 rounded-lg text-xs font-mono text-surface-700 overflow-x-auto">
            {newValues || 'None'}
          </pre>
        </div>
      </div>
    );
  }
}
