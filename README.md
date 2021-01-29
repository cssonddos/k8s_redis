A Redis cluster and a c# test app examples

---

# Deploying redis pods
## $ kubectl apply -f redis.yaml

# Checking cluster status of 1 pod
## $ kubectl exec -it redis-cluster-0 -- redis-cli cluster nodes
280b6865297194acf737c09ea86ecf498529dc06 :6379@16379 myself,master - 0 0 0 connected

# Configuring cluster with 1 master and 1 replica
## $ kubectl exec -it redis-cluster-0 -- redis-cli --cluster create --cluster-replicas 1 $(kubectl get pods -l app=redis-cluster -o jsonpath='{range.items[*]}{.status.podIP}:6379 ')
>>> Performing hash slots allocation on 6 nodes...
Master[0] -> Slots 0 - 5460
Master[1] -> Slots 5461 - 10922
Master[2] -> Slots 10923 - 16383
Adding replica 172.17.0.9:6379 to 172.17.0.6:6379
Adding replica 172.17.0.10:6379 to 172.17.0.7:6379
Adding replica 172.17.0.11:6379 to 172.17.0.8:6379
M: 280b6865297194acf737c09ea86ecf498529dc06 172.17.0.6:6379
   slots:[0-5460] (5461 slots) master
M: 0940bf62395db28bfa42f6f728c29f41c6cb2d96 172.17.0.7:6379
   slots:[5461-10922] (5462 slots) master
M: 341b5552c88b8499aa0ef97391de64e309839be8 172.17.0.8:6379
   slots:[10923-16383] (5461 slots) master
S: d2e0436309704d181014b60718070a37bddf5a5b 172.17.0.9:6379
   replicates 280b6865297194acf737c09ea86ecf498529dc06
S: e13a96b1c759cc63e577192d7db038c05fd4e6f1 172.17.0.10:6379
   replicates 0940bf62395db28bfa42f6f728c29f41c6cb2d96
S: 005b64884a1074f2141814417c53f3959d6f3dd2 172.17.0.11:6379
   replicates 341b5552c88b8499aa0ef97391de64e309839be8
Can I set the above configuration? (type 'yes' to accept): yes
>>> Nodes configuration updated
>>> Assign a different config epoch to each node
>>> Sending CLUSTER MEET messages to join the cluster
Waiting for the cluster to join
....
>>> Performing Cluster Check (using node 172.17.0.6:6379)
M: 280b6865297194acf737c09ea86ecf498529dc06 172.17.0.6:6379
   slots:[0-5460] (5461 slots) master
   1 additional replica(s)
S: d2e0436309704d181014b60718070a37bddf5a5b 172.17.0.9:6379
   slots: (0 slots) slave
   replicates 280b6865297194acf737c09ea86ecf498529dc06
M: 0940bf62395db28bfa42f6f728c29f41c6cb2d96 172.17.0.7:6379
   slots:[5461-10922] (5462 slots) master
   1 additional replica(s)
M: 341b5552c88b8499aa0ef97391de64e309839be8 172.17.0.8:6379
   slots:[10923-16383] (5461 slots) master
   1 additional replica(s)
S: e13a96b1c759cc63e577192d7db038c05fd4e6f1 172.17.0.10:6379
   slots: (0 slots) slave
   replicates 0940bf62395db28bfa42f6f728c29f41c6cb2d96
S: 005b64884a1074f2141814417c53f3959d6f3dd2 172.17.0.11:6379
   slots: (0 slots) slave
   replicates 341b5552c88b8499aa0ef97391de64e309839be8
[OK] All nodes agree about slots configuration.
>>> Check for open slots...
>>> Check slots coverage...
[OK] All 16384 slots covered.

# Checking cluster status of 1 pod
## $ kubectl exec -it redis-cluster-0 -- redis-cli cluster nodes
280b6865297194acf737c09ea86ecf498529dc06 172.17.0.6:6379@16379 myself,master - 0 1611747007000 1 connected 0-5460
d2e0436309704d181014b60718070a37bddf5a5b 172.17.0.9:6379@16379 slave 280b6865297194acf737c09ea86ecf498529dc06 0 1611747008885 4 connected

0940bf62395db28bfa42f6f728c29f41c6cb2d96 172.17.0.7:6379@16379 master - 0 1611747010917 2 connected 5461-10922
e13a96b1c759cc63e577192d7db038c05fd4e6f1 172.17.0.10:6379@16379 slave 0940bf62395db28bfa42f6f728c29f41c6cb2d96 0 1611747009000 5 connected

341b5552c88b8499aa0ef97391de64e309839be8 172.17.0.8:6379@16379 master - 0 1611747009000 3 connected 10923-16383
005b64884a1074f2141814417c53f3959d6f3dd2 172.17.0.11:6379@16379 slave 341b5552c88b8499aa0ef97391de64e309839be8 0 1611747009901 6 connected

# Checking cluster status of 1 pod
## $ kubectl exec -it redis-cluster-0 -- redis-cli cluster info
cluster_state:ok
cluster_slots_assigned:16384
cluster_slots_ok:16384
cluster_slots_pfail:0
cluster_slots_fail:0
cluster_known_nodes:6
cluster_size:3
cluster_current_epoch:6
cluster_my_epoch:1
cluster_stats_messages_ping_sent:820
cluster_stats_messages_pong_sent:817
cluster_stats_messages_sent:1637
cluster_stats_messages_ping_received:812
cluster_stats_messages_pong_received:820
cluster_stats_messages_meet_received:5
cluster_stats_messages_received:1637

# Deleting Master-0 and checking logs on his Slave (3)
## $ kubectl delete pod  redis-cluster-0
kubectl logs redis-cluster-3
sed: /data/nodes.conf: No such file or directory
1:C 27 Jan 2021 11:21:26.602 # oO0OoO0OoO0Oo Redis is starting oO0OoO0OoO0Oo
1:C 27 Jan 2021 11:21:26.602 # Redis version=5.0.1, bits=64, commit=00000000, modified=0, pid=1, just started
1:C 27 Jan 2021 11:21:26.602 # Configuration loaded
1:M 27 Jan 2021 11:21:26.605 * No cluster configuration found, I'm d2e0436309704d181014b60718070a37bddf5a5b
1:M 27 Jan 2021 11:21:26.611 * Running mode=cluster, port=6379.
1:M 27 Jan 2021 11:21:26.611 # WARNING: The TCP backlog setting of 511 cannot be enforced because /proc/sys/net/core/somaxconn is set to the lower value of 128.
1:M 27 Jan 2021 11:21:26.611 # Server initialized
1:M 27 Jan 2021 11:21:26.611 # WARNING you have Transparent Huge Pages (THP) support enabled in your kernel. This will create latency and memory usage issues with Redis. To fix this issue run the command 'echo never > /sys/kernel/mm/transparent_hugepage/enabled' as root, and add it to your /etc/rc.local in order to retain the setting after a reboot. Redis must be restarted after THP is disabled.
1:M 27 Jan 2021 11:21:26.611 * Ready to accept connections
1:M 27 Jan 2021 11:27:53.675 # configEpoch set to 4 via CLUSTER SET-CONFIG-EPOCH
1:M 27 Jan 2021 11:27:53.785 # IP address for this node updated to 172.17.0.9
1:S 27 Jan 2021 11:27:58.724 * Before turning into a replica, using my master parameters to synthesize a cached master: I may be able to synchronize with the new master with just a partial transfer.
1:S 27 Jan 2021 11:27:58.725 # Cluster state changed: ok
1:S 27 Jan 2021 11:27:59.267 * Connecting to MASTER 172.17.0.6:6379
1:S 27 Jan 2021 11:27:59.267 * MASTER <-> REPLICA sync started
1:S 27 Jan 2021 11:27:59.268 * Non blocking connect for SYNC fired the event.
1:S 27 Jan 2021 11:27:59.268 * Master replied to PING, replication can continue...
1:S 27 Jan 2021 11:27:59.272 * Trying a partial resynchronization (request 30532bc88ad87b13204d4446f7e5731ba1a5f01c:1).
1:S 27 Jan 2021 11:27:59.274 * Full resync from master: 93544a3e81c4ec56089b8ac78e835847bd5e3bb4:0
1:S 27 Jan 2021 11:27:59.274 * Discarding previously cached master state.
1:S 27 Jan 2021 11:27:59.382 * MASTER <-> REPLICA sync: receiving 175 bytes from master
1:S 27 Jan 2021 11:27:59.383 * MASTER <-> REPLICA sync: Flushing old data
1:S 27 Jan 2021 11:27:59.383 * MASTER <-> REPLICA sync: Loading DB in memory
1:S 27 Jan 2021 11:27:59.383 * MASTER <-> REPLICA sync: Finished with success
1:S 27 Jan 2021 11:27:59.384 * Background append only file rewriting started by pid 10
1:S 27 Jan 2021 11:27:59.419 * AOF rewrite child asks to stop sending diffs.
10:C 27 Jan 2021 11:27:59.420 * Parent agreed to stop sending diffs. Finalizing AOF...
10:C 27 Jan 2021 11:27:59.420 * Concatenating 0.00 MB of AOF diff received from parent.
10:C 27 Jan 2021 11:27:59.420 * SYNC append only file rewrite performed
10:C 27 Jan 2021 11:27:59.421 * AOF rewrite: 0 MB of memory used by copy-on-write
1:S 27 Jan 2021 11:27:59.469 * Background AOF rewrite terminated with success
1:S 27 Jan 2021 11:27:59.469 * Residual parent diff successfully flushed to the rewritten AOF (0.00 MB)
1:S 27 Jan 2021 11:27:59.469 * Background AOF rewrite finished successfully

# Manually promoting a slave node to master
## $ kubectl exec -it redis-cluster-3 -- redis-cli cluster failover
1:S 27 Jan 2021 11:50:58.657 # Connection with master lost.
1:S 27 Jan 2021 11:50:58.657 * Caching the disconnected master state.
1:S 27 Jan 2021 11:50:59.154 * Connecting to MASTER 172.17.0.5:6379
1:S 27 Jan 2021 11:50:59.154 * MASTER <-> REPLICA sync started
1:S 27 Jan 2021 11:51:02.208 # Error condition on socket for SYNC: Connection refused
1:S 27 Jan 2021 11:51:03.216 * Connecting to MASTER 172.17.0.5:6379
1:S 27 Jan 2021 11:51:03.218 * MASTER <-> REPLICA sync started
1:S 27 Jan 2021 11:51:03.221 * Non blocking connect for SYNC fired the event.
1:S 27 Jan 2021 11:51:03.222 * Master replied to PING, replication can continue...
1:S 27 Jan 2021 11:51:03.223 * Trying a partial resynchronization (request 1ba7ec60f275e493d58c4739ae1c795eea8cc738:379).
1:S 27 Jan 2021 11:51:03.227 * Full resync from master: 2818af2699a54c3cc6d86ba59757d223c13d9ed9:0
1:S 27 Jan 2021 11:51:03.227 * Discarding previously cached master state.
1:S 27 Jan 2021 11:51:03.280 * MASTER <-> REPLICA sync: receiving 750 bytes from master
1:S 27 Jan 2021 11:51:03.281 * MASTER <-> REPLICA sync: Flushing old data
1:S 27 Jan 2021 11:51:03.281 * MASTER <-> REPLICA sync: Loading DB in memory
1:S 27 Jan 2021 11:51:03.282 * MASTER <-> REPLICA sync: Finished with success
1:S 27 Jan 2021 11:51:03.282 * Background append only file rewriting started by pid 18
1:S 27 Jan 2021 11:51:03.313 * AOF rewrite child asks to stop sending diffs.
18:C 27 Jan 2021 11:51:03.313 * Parent agreed to stop sending diffs. Finalizing AOF...
18:C 27 Jan 2021 11:51:03.314 * Concatenating 0.00 MB of AOF diff received from parent.
18:C 27 Jan 2021 11:51:03.314 * SYNC append only file rewrite performed
18:C 27 Jan 2021 11:51:03.315 * AOF rewrite: 0 MB of memory used by copy-on-write
1:S 27 Jan 2021 11:51:03.322 * Background AOF rewrite terminated with success
1:S 27 Jan 2021 11:51:03.323 * Residual parent diff successfully flushed to the rewritten AOF (0.00 MB)
1:S 27 Jan 2021 11:51:03.323 * Background AOF rewrite finished successfully
1:S 27 Jan 2021 11:51:38.127 # Manual failover user request accepted.
1:S 27 Jan 2021 11:51:38.185 # Received replication offset for paused master manual failover: 42
1:S 27 Jan 2021 11:51:38.187 # All master replication stream processed, manual failover can start.
1:S 27 Jan 2021 11:51:38.187 # Start of election delayed for 0 milliseconds (rank #0, offset 42).
1:S 27 Jan 2021 11:51:38.290 # Starting a failover election for epoch 7.
1:S 27 Jan 2021 11:51:38.298 # Failover election won: I'm the new master.
1:S 27 Jan 2021 11:51:38.298 # configEpoch set to 7 after successful failover
1:M 27 Jan 2021 11:51:38.298 # Setting secondary replication ID to 2818af2699a54c3cc6d86ba59757d223c13d9ed9, valid up to offset: 43. New replication ID is 0314e3cdb4f9058aad15d98ac4f646e807874499
1:M 27 Jan 2021 11:51:38.298 # Connection with master lost.
1:M 27 Jan 2021 11:51:38.298 * Caching the disconnected master state.
1:M 27 Jan 2021 11:51:38.298 * Discarding previously cached master state.
1:M 27 Jan 2021 11:51:38.393 * Replica 172.17.0.5:6379 asks for synchronization
1:M 27 Jan 2021 11:51:38.393 * Partial resynchronization request from 172.17.0.5:6379 accepted. Sending 0 bytes of backlog starting from offset 43.

## $ kubectl logs redis-cluster-0
1:C 27 Jan 2021 11:51:02.851 # oO0OoO0OoO0Oo Redis is starting oO0OoO0OoO0Oo
1:C 27 Jan 2021 11:51:02.851 # Redis version=5.0.1, bits=64, commit=00000000, modified=0, pid=1, just started
1:C 27 Jan 2021 11:51:02.851 # Configuration loaded
1:M 27 Jan 2021 11:51:02.856 * Node configuration loaded, I'm 280b6865297194acf737c09ea86ecf498529dc06
1:M 27 Jan 2021 11:51:02.858 * Running mode=cluster, port=6379.
1:M 27 Jan 2021 11:51:02.858 # WARNING: The TCP backlog setting of 511 cannot be enforced because /proc/sys/net/core/somaxconn is set to the lower value of 128.
1:M 27 Jan 2021 11:51:02.858 # Server initialized
1:M 27 Jan 2021 11:51:02.858 # WARNING you have Transparent Huge Pages (THP) support enabled in your kernel. This will create latency and memory usage issues with Redis. To fix this issue run the command 'echo never > /sys/kernel/mm/transparent_hugepage/enabled' as root, and add it to your /etc/rc.local in order to retain the setting after a reboot. Redis must be restarted after THP is disabled.
1:M 27 Jan 2021 11:51:02.860 * DB loaded from append only file: 0.002 seconds
1:M 27 Jan 2021 11:51:02.860 * Ready to accept connections
1:M 27 Jan 2021 11:51:03.224 * Replica 172.17.0.9:6379 asks for synchronization
1:M 27 Jan 2021 11:51:03.224 * Partial resynchronization not accepted: Replication ID mismatch (Replica asked for '1ba7ec60f275e493d58c4739ae1c795eea8cc738', my replication IDs are '6d8faf21c1d6f8f552b8a2c2aaa4ef4bcf59cfd5' and '0000000000000000000000000000000000000000')
1:M 27 Jan 2021 11:51:03.224 * Starting BGSAVE for SYNC with target: disk
1:M 27 Jan 2021 11:51:03.225 * Background saving started by pid 11
11:C 27 Jan 2021 11:51:03.238 * DB saved on disk
11:C 27 Jan 2021 11:51:03.238 * RDB: 0 MB of memory used by copy-on-write
1:M 27 Jan 2021 11:51:03.278 * Background saving terminated with success
1:M 27 Jan 2021 11:51:03.278 * Synchronization with replica 172.17.0.9:6379 succeeded
1:M 27 Jan 2021 11:51:04.907 # Cluster state changed: ok
1:M 27 Jan 2021 11:51:38.127 # Manual failover requested by replica d2e0436309704d181014b60718070a37bddf5a5b.
1:M 27 Jan 2021 11:51:38.294 # Failover auth granted to d2e0436309704d181014b60718070a37bddf5a5b for epoch 7
1:M 27 Jan 2021 11:51:38.299 # Connection with replica 172.17.0.9:6379 lost.
1:M 27 Jan 2021 11:51:38.304 # Configuration change detected. Reconfiguring myself as a replica of d2e0436309704d181014b60718070a37bddf5a5b
1:S 27 Jan 2021 11:51:38.304 * Before turning into a replica, using my master parameters to synthesize a cached master: I may be able to synchronize with the new master with just a partial transfer.
1:S 27 Jan 2021 11:51:38.386 * Connecting to MASTER 172.17.0.9:6379
1:S 27 Jan 2021 11:51:38.387 * MASTER <-> REPLICA sync started
1:S 27 Jan 2021 11:51:38.388 * Non blocking connect for SYNC fired the event.
1:S 27 Jan 2021 11:51:38.389 * Master replied to PING, replication can continue...
1:S 27 Jan 2021 11:51:38.392 * Trying a partial resynchronization (request 2818af2699a54c3cc6d86ba59757d223c13d9ed9:43).
1:S 27 Jan 2021 11:51:38.394 * Successful partial resynchronization with master.
1:S 27 Jan 2021 11:51:38.394 # Master replication ID changed to 0314e3cdb4f9058aad15d98ac4f646e807874499
1:S 27 Jan 2021 11:51:38.394 * MASTER <-> REPLICA sync: Master accepted a Partial Resynchronization.
1:S 27 Jan 2021 14:23:28.780 # MASTER timeout: no data nor PING received...
1:S 27 Jan 2021 14:23:28.781 # Connection with master lost.
1:S 27 Jan 2021 14:23:28.781 * Caching the disconnected master state.
1:S 27 Jan 2021 14:23:28.781 * Connecting to MASTER 172.17.0.9:6379
1:S 27 Jan 2021 14:23:28.783 * MASTER <-> REPLICA sync started
1:S 27 Jan 2021 14:23:28.784 * Non blocking connect for SYNC fired the event.
1:S 27 Jan 2021 14:23:28.787 * Master replied to PING, replication can continue...
1:S 27 Jan 2021 14:23:28.798 * Trying a partial resynchronization (request 0314e3cdb4f9058aad15d98ac4f646e807874499:11733).
1:S 27 Jan 2021 14:23:28.800 * Successful partial resynchronization with master.
1:S 27 Jan 2021 14:23:28.801 * MASTER <-> REPLICA sync: Master accepted a Partial Resynchronization.
1:S 27 Jan 2021 16:23:15.948 # MASTER timeout: no data nor PING received...
1:S 27 Jan 2021 16:23:15.948 # Connection with master lost.
1:S 27 Jan 2021 16:23:15.948 * Caching the disconnected master state.
1:S 27 Jan 2021 16:23:15.948 * Connecting to MASTER 172.17.0.9:6379
1:S 27 Jan 2021 16:23:15.949 * MASTER <-> REPLICA sync started
1:S 27 Jan 2021 16:23:15.950 * Non blocking connect for SYNC fired the event.
1:S 27 Jan 2021 16:23:15.953 * Master replied to PING, replication can continue...
1:S 27 Jan 2021 16:23:15.960 * Trying a partial resynchronization (request 0314e3cdb4f9058aad15d98ac4f646e807874499:16815).
1:S 27 Jan 2021 16:23:15.963 * Successful partial resynchronization with master.
1:S 27 Jan 2021 16:23:15.963 * MASTER <-> REPLICA sync: Master accepted a Partial Resynchronization.