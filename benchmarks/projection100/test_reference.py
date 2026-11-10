"""Check projection semantics with independently understandable small event streams."""
import unittest
import run as projection

class ProjectionReferenceTests(unittest.TestCase):
 def test_ten_domains_ten_kinds(self):
  r=projection.reference(100,1,'fanout')
  self.assertEqual(r['stateCount'],10)
  self.assertEqual(r['retainedItems'],10)
  self.assertEqual(r['maxHistoryLength'],1)
  self.assertEqual(r['ignored'],910)
 def test_dense_history_and_wide_entities(self):
  dense=projection.reference(1000,1,'fanout');wide=projection.reference(1000,10,'fanout')
  self.assertEqual((dense['stateCount'],dense['retainedItems'],dense['maxHistoryLength']),(10,100,10))
  self.assertEqual((wide['stateCount'],wide['retainedItems'],wide['maxHistoryLength']),(100,100,1))
 def test_routing_changes_only_ignored_count(self):
  fanout=projection.reference(1000,2,'fanout');routed=projection.reference(1000,2,'domain')
  self.assertEqual(fanout['sink'],routed['sink'])
  self.assertEqual(routed['ignored'],10)
  self.assertEqual(fanout['ignored'],9010)

if __name__=='__main__':unittest.main()
